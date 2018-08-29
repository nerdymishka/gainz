function Import-KeePassBinary() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.IKeePassPackage] $Package,

        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.IKeePassEntry] $Entry,

     
        [string] $Path,

        [Parameter(Mandatory = $True, Position = 0)]
        [String] $Name,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [Byte[]] $Data 
    )

    if($null -eq $Entry -and $null -eq $Package) {
        throw [System.ArgumentNullException] "Both Entry and Package can not be null."
    }

    if($Entry) {
        $Package = $Entry.Owner;
    } else {
        if([string]::IsNullOrWhiteSpace($Path)) {
            throw [System.ArgumentNullException] "Path"
        }

        $Entry = $Package.FindEntry($Path);
    }

   
    if($null -eq $entry) {
        Write-Warning "Could not locate $Path"
        return $null;
    }

    $Package.AttachBinary($entry, $Name, $Data)
}