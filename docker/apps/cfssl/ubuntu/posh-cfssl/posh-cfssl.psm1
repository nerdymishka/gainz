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
    $cfg = Read-Config
    $dbConfig =$cfg.db;

    $dbEnvironment = $cfg.environment;


    $config = Get-Content  ($dbConfig) -Raw | ConvertFrom-Json;

    $migrationFile = $drivers[$config.driver];
    $cmd = "down";
    if([string]::IsNullOrWhiteSpace($Direction) -or $Direction -eq "Up") {
         $cmd = "up"
    }

    & goose -env $dbEnvironment -path $migrationFile $cmd
}



function New-CfsslCsr() {
    Param(
        [PsCustomObject] $Csr,
        [String] $Kind = "default"
    )
    
    if(!$Csr.Names) {
        $Csr | Add-Member -Name "names" -Value @(
            [PSCustomObject]@{
                L = $Env:CFSSL_DEFAULT_LOCALITY
                S = $Env:CFSSL_DEFAULT_STATE
                C = $ENV:CFSSL_DEFAULT_COUNTRY
                O = $ENV:CFSSL_DEFAULT_ORG 
                OU = $ENV:CFSSL_DEFAULT_ORG_UNIT
            }
        )
    }

    if(!$Csr.CN) {
        switch($kind) {
            "ca"  {
                $csr.CN = "$($ENV:CFSSL_DEFAULT_CN) ROOT CA";
            }
            "intermediate"  {
                $csr.CN = "$($ENV:CFSSL_DEFAULT_CN) INTERMEDIATE CA";
            }
            "oscp" {
                $csr.CN = "$($ENV:CFSSL_DEFAULT_CN) OSCP Signer";
               
            }
            default {
                $csr.CN = $ENV:CFSSL_DEFAULT_CN;
            }
        }
    }
    if(!$CSR.key) {
        switch(kind) {
            "ca" {
                $csr | Add-Member -Name "key" -Value ([PSCustomObject]@{
                    algo = "ecdsa"
                    size = 256
                });
            }
            "intermediate" {
                $csr | Add-Member -Name "key" -Value ([PSCustomObject]@{
                    algo = "ecdsa"
                    size = 256
                });
            }
            "oscp" {
                $csr | Add-Member -Name "key" -Value ([PSCustomObject]@{
                    algo = "ecdsa"
                    size = 256
                });
            }
            default {
                $csr | Add-Member -Name "key" -Value ([PSCustomObject]@{
                    algo = "rsa"
                    size = 2048
                });
            }
        }
    }

    return ConvertTo-Json $csr;
}

function ConvertTo-HexString() {
    Param(
        [Parameter(Position = 0)]
        [String] $Value
    )

    if([string]::IsNullOrWhiteSpace($Value)) {
        return $null;
    }

    $result = $Value | Format-Hex
    return [System.BitConverter]::ToSingle($result.Bytes).Replace("-", "")
}

$cfg = $null;

function Initialize-CfsslServer() {

    Update-CfsslConfigDefaults
    $cfg = Read-Config
    $ca = $cfg.csr.ca | Where-Object { $_.default -eq $true }
    if($ca -is [Array]) {
        Write-Error "there can only be one default ca";
        return;
    } 
    
    $name = $ca.name 
    $path = $ca.path
    if(!(Test-Path "/etc/cfssl/$name.pem")) {
        Write-Host "creating CA cert" -ForegroundColor Green
        & cfssl gencert -initca $path | cfssljson -bare $name 
    } 
    

    $ca = $name;
    $intermediate = $cfg.csr.intermediate | Where-Object { $_.default -eq $true }
    if($intermediate -is [Array]) {
        Write-Error "there can only be one default intermediate-ca";
        return;
    }

    $name = $intermediate.name 
    $path = $intermediate.path  
    $config = $cfg.config 
    if(!(Test-Path $config)) {
        Write-Warning "Could not locate $config";
    }
    if(!(Test-Path "/etc/cfssl/$name.pem")) {
        Write-Host "Creating Intermediate CA cert" -ForegroundColor Green
        & cfssl gencert -initca -config $config -profile="intermediate" $path | cfssljson -bare $name
        Write-Host "Signing Intermediate CA cert" -ForegroundColor Green 
        & cfssl sign -ca "$ca.pem" -ca-key "$ca-key.pem" -config $config -profile="intermediate" "/etc/cfssl/$name.csr" | cfssljson -bare $name
    }
    $oscp = $cfg.csr.oscp | Where-Object { $_.default -eq $true } 

    if(!(Test-Path "/etc/cfssl/$($oscp.name).pem")) {
        Write-Host "creating oscp cert" -ForegroundColor Green
        & cfssl gencert -ca "$name.pem" -ca-key "$name-key.pem" -config $config -profile="ocsp" $($oscp.path) | cfssljson -bare $($oscp.name) 
    }

    # helpful for openssl or the need for certificate chains
    cat "$ca.pem" "$name.pem" > "ca-chain.pem"

    if("$($ENV:CFSSL_DB_INIT)" -eq "1") {
        New-CfsslDatabase -Direction "Up"
    }
}

function Read-Config() {
   

    if($cfg) {
        return $cfg;
    }


    $cfg = "/etc/cfssl/image-config.json"
    $cfg = Get-Content $cfg -Raw 
    $cfg = ConvertFrom-Json $cfg;

    return $cfg;
}

function Update-CfsslConfigDefaults() {

    $cfg = Read-Config

    if(!$cfg.update) {
        Write-Debug "update is false";
        return;
    } 

    $requests = $cfg.csr;
    
    $ca = $requests.ca;
    if(!($ca -is [array])) {
        $ca = @($ca)
    }

    $intermediate = $requests.intermediate;
    if(!($intermediate -is [array])) {
        $intermediate = @($intermediate)
    }

    $oscp = $requests.oscp;
    if(!($oscp -is [array])) {
        $oscp = @($oscp)
    }

    function Set-Defaults() {
        Param(
            [PsCustomObject] $Csr 
        )

        foreach($n in $Csr.names) {
            $n.C = $ENV:CFSSL_DEFAULT_COUNTRY
            $n.S = $Env:CFSSL_DEFAULT_STATE
            $n.L = $Env:CFSSL_DEFAULT_LOCALITY
            $n.O = $Env:CFSSL_DEFAULT_ORG
            $n.OU = $Env:CFSSL_DEFAULT_ORG_UNIT 
        }

        $Csr.CN = $Csr.CN.Replace("ACME", $Env:CFSSL_DEFAULT_CN)
    }

    foreach($cert in $ca) {
        $json = Get-Content $cert -Raw 
        $json = ConvertFrom-Json $json 

        Set-Defaults -Csr $json 

        $json = ConvertTo-Json $json 
        $json | Out-File $cert -Force 
    }

    foreach($cert in $ca) {
        $json = Get-Content $cert -Raw 
        $json = ConvertFrom-Json $json 

        Set-Defaults -Csr $json 

        $json = ConvertTo-Json $json 
        $json | Out-File $cert -Force 
    }

    foreach($cert in $intermediate) {
        $json = Get-Content $cert -Raw 
        $json = ConvertFrom-Json $json 

        Set-Defaults -Csr $json 

        $json = ConvertTo-Json $json 
        $json | Out-File $cert -Force 
    }

    foreach($cert in $oscp) {
        $json = Get-Content $cert -Raw 
        $json = ConvertFrom-Json $json 

        Set-Defaults -Csr $json 

        $json = ConvertTo-Json $json 
        $json | Out-File $cert -Force 
    }
}

Export-ModuleMember -Function @(
    "Initialize-CfsslServer",
    "Read-Config"
)