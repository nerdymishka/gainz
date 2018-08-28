function Import-KeePassBinary() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.IKeePassPackage] $Package,

        [Parameter(Position = 0)]
        [string] $Path,

        [String] $Name,
        
        [Byte[]] $Data 
    )

    $entry = $Package.FindEntry($Path)
    if($entry -eq $null) {
        Write-Warning "Could not locate $Path"
        return null;
    }

    $Package.AttachBinary($entry, $Name, $Data)
}