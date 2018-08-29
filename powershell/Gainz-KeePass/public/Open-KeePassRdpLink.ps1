
function Open-KeePassRdpLink() {
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
    if($fields.Url -and $fields.UserName) {
        $url = $fields.Url;
        $pw = $entry.Fields.UnprotectPassword();
        if($url -match "://") {
            $index = $url.IndexOf("://");
            $url = $url.substring($index + 3);
        }
        cmdkey.exe /generic:TERMSRV/$url /user:$($fields.UserName) /password:$pw 
        Start-Process -FilePath "$Env:Windir\system32\mstsc.exe" -ArgumentList "/v:$url" -Wait
        cmdkey.exe /delete:TERMSRV/$url 
    }
}