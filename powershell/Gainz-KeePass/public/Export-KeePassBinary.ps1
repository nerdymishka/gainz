function Export-KeePassBinary() {
<#
    .SYNOPSIS 
    Exports a binary file from a KeePass Entry object

    .DESCRIPTION
    This method will write the binary information associated with
    a KeePass entry. Each association is stored with the file's 
    name and extension e.g. cert.pfx

    .PARAMETER Entry
    The KeePass Entry object the file is attached to.

    .PARAMETER Name
    The file name the binary data was stored as.

    .PARAMETER DestinationPath
    The file path where the exported file should be saved to. The `Dest`
    parameter is an alias.

    .PARAMETER Force
    (Optional) This will force this command to overwrite the file at the
    DestinationPath if it exists.

    .EXAMPLE
    $entry | Export-KeePassBinary -Name "azure.pfx" -DestinationPath "$home/Desktop/azure.pfx"

    .EXAMPLE
    PS C:\> $key | Open-KeePassPackage "$home/Desktop/passwords.kdbx" -Do {
    PS C:\>     $entry = $_ | Find-KeePassEntryByTitle "cert:azure"
    PS C:\>     $entry | Export-KeePassBinary -Name "azure.pfx" -DestinationPath "$home/Desktop/azure.pfx"
    PS C:\>     Write-Host ($entry.UnprotectPassword())
    PS C:\> }
#>
    Param(
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.IKeePassEntry] $Entry,

        [Parameter(Mandatory = $true, Position = 0)]
        [string] $Name,
        [Parameter(Mandatory = $true, Position = 1)]
        [Alias("Dest")]
        [string] $DestinationPath,
        [switch] $Force
    )

    $rootDir = $DestinationPath
    $fileName = $Name 
    if([System.IO.Path]::HasExtension($DestinationPath)) {
        $rootDir = Split-Path $DestinationPath
        $fileName = Split-Path $DestinationPath -Leaf
    }

   
    if(!(Test-Path $rootDir)) {
        if(!$Force.ToBool()) {
            Write-Warning "$rootDir does not exist"
            return $null;
        } else {
            New-Item $rootDir -ItemType Directory -Force
        }
    }
    

   
    
    $binary = $null;
    foreach($bin in $Entry.Binaries) {
        if($bin.Key -eq $Name) {
            $binary = $bin;
            break;
        }
    }

    if($binary) {
        $data = $binary.Value.UnprotectAsBytes()
        $file = "$rootDir/$fileName"
        [IO.File]::WriteAllBytes($file, $data)
    }
}