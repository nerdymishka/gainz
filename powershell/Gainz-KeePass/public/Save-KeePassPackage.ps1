function Save-KeePassPackage() {
    Param(
        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.KeePassPackage] $Package,
        
        [NerdyMishka.KeePass.MasterKey] $Key,

        [Parameter(Position = 0)]
        [String] $Path,

        [Switch] $Force 
    )

    if(!$Force.ToBool() -and (Test-Path $Path)) {
        Write-Warning "File already exists $Path";
        return null; 
    }

    $directoryName = [IO.Path]::GetDirectoryName($Path);
    
    if(!(Test-Path $directoryName)) {
        New-Item $directoryName -ItemType Directory -Force
    }

    

    [NerdyMishka.KeePass.IKeePassPackageSerializer]$serializer = (New-Object NerdyMishka.KeePass.Xml.KeePassPackageXmlSerializer)

    if($Key -eq $null) {
        $Key = $Package.MasterKey;
    }
    $fs = [IO.File]::OpenWrite($Path);
    $result = $Package.Save($key, $fs, $serializer);
    $fs.Dispose()

    return $result;
}