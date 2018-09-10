function New-CfsslDatabase() {
    Param(
        [PArameter(Position = 0)]
        [String] $Direction = "Up"
    )

    $certDbPath = "${Env:GOPATH}/src/github.com/cloudflare/cfssl/certdb/"

    $drivers = @{
        "sqlite3" = "$certDbPath/sqlite"
        "postgres" = "$certDbPath/pg"
        "mysql" = "$certDbPath/mysql"
    }
    $dbConfig = $Env:CFSSL_DB_CONFIG

    $dbEnvironment = $Env:CFSSL_DB_ENVIRONMENT
    if(!$dbConfig) {
        $dbConfig = "cfssl-db.json"
    }

    if(!$dbEnvironment) {
        $dbEnvironment = "Production"
    }

    $config = Get-Content  ("/etc/cfssl/" + $dbConfig) -RAw | ConvertFrom-Json;
    $migrationFile = $drivers[$config.driver];
    $cmd = "down";
    if([string]::IsNullOrWhiteSpace($Direction) -or $Direction -eq "Up") {
         $cmd = "up"
    }

    & goose -env $dbEnvironment -path $migrationFile $cmd
}

function New-RootCa() {
    Param(
        [Parameter(Position = 0)]
        [String] $Csr = $null
    )

    $csr = $RootCaCsr;
    if(![string]::IsNullOrWhiteSpace($csr)) {
        if(! (Test-Path $csr)) {
            Write-Debug "Could not location $csr falling back to /etc/cfssl/$($Env:CFSSL_CSR)"
            $csr = "/etc/cfssl$($Env:CFSSL_CSR)"
        }
    }

    if(!(Test-Path $csr)) {
        Write-Error "Cannot locate $csr";
        return 
    }

    & cfssl gencert -initca $csr | cfssl -bare ca

   
}

function New-IntermediateCa() {
    Param(
        [Parameter(Position = 0)]
        [String] $Csr = $null
    )

    if(![string]::IsNullOrWhiteSpace($csr)) {
        if(! (Test-Path $csr)) {
            Write-Debug "Could not location $csr falling back to /etc/cfssl/$($Env:CFSSL_CSR)"
            $csr = "/etc/cfssl$($Env:CFSSL_CSR)"
        }
    }

    if(!(Test-Path $csr)) {
        Write-Error "Cannot locate $csr";
        return 
    }

    & cfssl gencert -initca $csr | cfssl -bare ca
    $response = (Invoke-WebRequest -Uri "$($ENV:CFSSL_URI)/api/v1/cfssl/info").Content

    $dot = "."
    while(!$response -match "primary") {
        $dot += "."
        Write-Host "`rRoot CA is not available$dot" -NoNewline
        Start-Sleep 5 
        $response = (Invoke-WebRequest -Uri "$($ENV:CFSSL_URI)/api/v1/cfssl/info").Content
    }

    $Csr=$(awk '{printf "%s\\n", $0}' ca.csr)
    $Data= @{"certificate_request" = "'$Csr'"};
    $response = (Invoke-WebRequest -Uri  "$($ENV:CFSSL_URI)/api/v1/cfssl/sign" -Method Post -Body $data).Content
    $response = $Response | ConvertFrom-Json
    $cert = $response["result"]["certificate"]
    $cert | Out-file "/etc/cfssl/ca.pem"
}