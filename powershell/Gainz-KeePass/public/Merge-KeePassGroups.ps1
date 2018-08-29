
function Merge-KeePassGroups () {
    Param(
        [Parameter(ValueFromPipeline = $True)]
        [NerdyMishak.KeePass.IKeePassPackage] $Package,

        [Parameter(Position = 0)]
        [String] $Path,
        
        [Parameter(Position = 1)]
        [String] $GroupName,
        
        [Switch] $Force 
    )

    $srcPackage = $Package | Open-KeePassKdbxLink $Path
    
    $srcGroup = $srcPackage.FindGroup($Path);
    if($srcGroup -eq $null) {
        Write-Warning "Could not find Group from source package: $Path"
        return;
    }
    $destGroup = $Package.FindGroup($Path);
    $srcGroup.MergeTo($destGroup, $Force.ToBool())
   

    $srcPackage.Dispose();

}