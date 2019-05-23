
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

function Connect-SftpClient() {
    Param(
        [String] $HostName,

        [String] $UserName,

        [Int32] $Port = 22,
        
        [SecureString] $Password,

        [Renci.SshNet.PrivateKeyFile[]] $PrivateKeys 
    )

    $splat = @{
        "HostName" = $HostName
        "UserName" = $UserName
        "Port" = $Port 
        "Password" = $Password
        "PrivateKeys" = $PrivateKeys
    }

    $Client = New-SftpClient @splat 
    $Client.Connect()
    return $Client;
}

function Disconnect-SftpClient() {
    param(
        [Parameter(ValueFromPipeline = $True, ValueFromPipelineByPropertyName = $true)]
        [Renci.SshNet.SftpClient] $Client
    )

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