
#NET 4.0 libs

if($PSEdition -eq "Desktop") {
   
    Add-Type -Path "$PSScriptRoot\bin\SshNet.Security.Cryptography.dll"
    Add-Type -Path "$PSScriptRoot\bin\Renci.SshNet.dll"
} else {
    throw "Core is not supported yet."
}



#TODO: .NET CORE
if($null -eq (Get-Command ConvertTo-UnprotectedBytes -EA SilentlyContinue)) {

    function ConvertTo-UnprotectedBytes() {
        Param(
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [SecureString] $SecureString,

            [String] $Encoding = "UTF-8"
        )



        $enc = [System.Text.Encoding]::GetEncoding($Encoding)
        

        $bstr = [IntPtr]::Zero;
        $charArray = New-Object 'char[]' -ArgumentList $SecureString.Length

        try
        {

            $bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecureString);
            [System.Runtime.InteropServices.Marshal]::Copy($bstr, $charArray, 0, $charArray.Length);

            $bytes = $enc.GetBytes($charArray);
            return $bytes
        }
        finally
        {
            [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr);
        }
    }
}

function Get-SftpDefaultClient() {


    if($gzSftpClients.ContainsKey("Default")) {
        return $gzSftpClients["Default"];
    }

    return $null;
}

function New-SftpClient() {
    Param(
        [String] $HostName,

        [String] $UserName,

        [Int32] $Port = 22,
        
        [SecureString] $Password,

        [Renci.SshNet.PrivateKeyFile[]] $PrivateKeys 
    )

    $info = $null;

    if(!$Password -and !$PrivateKeys) {
        throw "Either a password or a private key must be specified"
    }

    if($Password)
    {
        $PasswordBytes = ConvertTo-UnprotectedBytes $Password
        $info = New-Object Renci.SshNet.PasswordConnectionInfo($HostName, $Port, $UserName, [byte[]] $PasswordBytes)
    } else {
        $info = New-Object Renci.SshNet.PrivateKeyConnectionInfo -ArgumentList $HostNAme, $Port, $UserName, $PrivateKeys 
    }

    $client = New-Object Renci.SshNet.SftpClient($info) 
    return $client;
}

$gzSftpClients = @{}

function Register-SftpClient() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name = "Default",

        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client
    )

    if($gzSftpClients.ContainsKey($Name)) {
        $gzSftpClients[$Name].Dispose()
        Write-Debug "Disposed the current $Name SFTP Client"
    }

    $gzSftpClients[$Name] = $Client;
}

function Unregister-SftpClient() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name = "Default",

        [Switch] $PassThru,

        [Switch] $Dispose 
    )

    if($gzSftpClients.ContainsKey($name)) {
        $client = $gzSftpClients[$name]

        $gzSftpClients.Remove($Name)
        if($Dispose.ToBool()) {
            $client.Dispose()
        }

        if($PassThru.ToBool()) {
            return $client;
        }
    }
}



function Connect-SftpClient() {
    Param(
        [String] $HostName,

        [String] $UserName,

        [Int32] $Port = 22,
        
        [SecureString] $Password,

        [Renci.SshNet.PrivateKeyFile[]] $PrivateKeys,

        [Switch] $Default
    )

    $splat = @{
        "HostName" = $HostName
        "UserName" = $UserName
        "Port" = $Port 
        "Password" = $Password
        "PrivateKeys" = $PrivateKeys
    }

    $Client = New-SftpClient @splat 

    if($Default.ToBool()) {
        $Client | Register-SftpClient
    }

    $Client.Connect()
    return $Client;
}

function Disconnect-SftpClient() {
    param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client
    )

    if(!$Client) {
        $Client = Get-SftpDefaultClient

        if(!$Client) {
            throw  "Either a default SFTP client must be set or the -Client parameter must be specified"
        }
    }

    $Client.Disconnect()
}


function Read-SftpDirectory() {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client,

        [Parameter(Position = 0)]
        [String] $Path 
    )

    if([string]::IsNullOrWhiteSpace($Path)) {
        $Path = $Client.WorkingDirectory
    }

    Write-Output $Client.ListDirectory($Path) -NoEnumerate
}

function Set-SftpDirectory() {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Path 
    )

    if(!$Client) {
        $Client = Get-SftpDefaultClient

        if(!$Client) {
            throw  "Either a default SFTP client must be set or the -Client parameter must be specified"
        }
    }

    $Client.ChangeDirectory($Path)
}

function Get-SftpFile {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Path
    )

    if(!$Client) {
        $Client = Get-SftpDefaultClient

        if(!$Client) {
            throw  "Either a default SFTP client must be set or the -Client parameter must be specified"
        }
    }

    return $Client.Get($Path)
}

function Test-SftpPath {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Path
    )

    if(!$Client) {
        $Client = Get-SftpDefaultClient

        if(!$Client) {
            throw  "Either a default SFTP client must be set or the -Client parameter must be specified"
        }
    }

    return $Client.Exists($Path)
}

function Set-SftpFilePermission {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Path, 
        
        [Parameter(Position = 1)]
        [Int16] $Mode  
    )

    if(!$Client) {
        $Client = Get-SftpDefaultClient

        if(!$Client) {
            throw  "Either a default SFTP client must be set or the -Client parameter must be specified"
        }
    }

    $file = $Client.Get($Path)
    $file.SetPermissions($Mode)
}

function Invoke-SftpDownload() {
    Param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1)]
        [String] $OutFile,

        [IO.Stream] $Stream,

        [Parameter(ValueFromRemainingArguments)]
        [ScriptBlock] $Callback,

        [Switch] $Force
    )

    if(!$Client) {
        $Client = Get-SftpDefaultClient

        if(!$Client) {
            throw  "Either a default SFTP client must be set or the -Client parameter must be specified"
        }
    }

    $isEmpty = [String]::IsNullOrWhiteSpace($OutFile)

    if($isEmpty -and !$Stream) {
        throw "Stream or OutFile must be specified"
    }

    if(!$isEmpty) {
        $mode = [System.IO.FileMode]::CreateNew
        if($Force.ToBool()) {
            $mode = [System.IO.FileMode]::Create
        }
        $rw = [System.IO.FileAccess]::ReadWrite
        $share = [System.IO.FileShare]::None
        $Stream = [System.IO.FileStream]::new($OutFile, $mode, $rw, $share, 4096, $true)
    }

    
   

    if($Callback) {
        $Client.DownloadFile($Path, $Stream, $Callback)
        return;
    }

    $file = $Client.Get($Path)
    $total = $file.Length 
    # TODO: get this working to see progress, I may need to write extensions
    <#
    [Action[uint64]]{
        Param(
            [UInt64] $byteCount
        )
        <#
        $x = ($byteCount / $total) * 100
        $x = [Math]::Round($x)
        if($x -lt 10) {
            $x = "0" + $x.ToString()
        }
        Write-Host "$x% Downloaded: $byteCount/$total"  -NoNewline 
    }#>

    

    $Client.DownloadFile($Path, $Stream)
}

function Invoke-SftpTest() {
    Param(
        [String] $Host,
        [String] $UserName,
        [String] $Pass
    )
    $pw = ConvertTo-SecureString $Pass  -AsPlainText -Force
    $client = Connect-SftpClient -hostName $Host -username $UserName -Password $pw
   
    $client | Read-SftpDirectory
    $client.Dispose()
}