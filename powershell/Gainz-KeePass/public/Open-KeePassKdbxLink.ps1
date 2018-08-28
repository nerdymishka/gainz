function Open-KeePassKdbxLink() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.IKeePassEntry] $Entry,

        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.IKeePassPackage] $Package
    )

    if($entry -eq $null -and $Package) {
        $entry = $Package.FindEntry($Path);
    }

    if($entry -eq $null) {
        Write-Warning "Could not find $entry at $Path";
    }
    $fields = $entry.Fields;

    if([string]::IsNullOrWhiteSpace($fields.Url)) {
        Write-Warning "Url missing from entry ${fields.Title}"
        return null;
    }

  
    $url = $fields.Url;
    $pw = $entry.Fields.UnprotectPassword();
    if($url -match "://") {
        $index = $url.IndexOf("://");
        $url = $url.substring($index + 3);
    }
    
    return Open-KeePassPackage $url -Key -Password $pw 
}