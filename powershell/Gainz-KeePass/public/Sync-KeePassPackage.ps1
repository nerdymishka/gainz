
function Sync-KeePassPackage() {
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string] $Path,
        
        [Parameter(ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.MasterKey] $Key,
        
        [SecureString] $Password = $null,
        
        [String] $KeyFile = $null ,
        
        [Switch] $UserAccount,

        [string] $Source,

        [string] $KeyRingPath,

        [switch] $MergePackage,

        [string] $GroupName
    )

    $package = Open-KeePassPackage -Path $Path -Key $Key -Password $Password -KeyFile $KeyFile 

    $fileName = Split-Path $Source -Leaf 
    
    if([string]::IsNullOrWhiteSpace($KeyRingPath)) {
        $root = $package.Root.Name
        $KeyRingPath = "$root/keyring" 
    }

    $entry =  $package.FindEntry($KeyRingPath)
    if(!$entry) {
        Write-Warning "Could not find source in key ring path $KeyRingPath"
        $package.Dispose();
        return;
    }

    $credential = $entry.UnprotectPasswordAsSecureString()

    $sourcePackage = Open-KeePassPackage -Path $Source -Password $credential

    if($MergePackage.ToBool()) {
        $sourcePackage.Merge($package);
        $package | Save-KeePassPackage 
        $package.Dispose()
        $sourcePackage.Dispose()
        $packageName = Split-Path $path -Leaf
        Write-Host "$fileName synced to $packageName"
        return;
    }

    if([string]::IsNullOrEmpty($GroupName)) {       
        $GroupName = $sourcePackage.Root.Name 
        Write-Debug "Group was not specified, using the root node: $GroupName"
    }

    $group = $package.FindGroup($GroupName)
    $packageName = Split-Path Path -Leaf
    if($group -eq $null) {
        
        $answer =  Read-Host -Prompt "Group $GroupName does not exist in $packageName. Create? Y = yes"
        if($answer -ne "y") {
            Write-Warning "aborting sync"
            $package.Dispose()
            $sourcePackage.Dispose();
            return;
        }
        $group = $package.CreateGroup($GroupName)
    }
    $sourceGroup = $sourcePackage.FindGroup($GroupName);
    if($sourceGroup -eq $null) {
        Write-Warning "Group $GroupName not found in source"
        $sourcePackage.Dispose();
        $package.Dispose();
        return;
    }
    $sourceGroup.MergeTo($group);
    $package | Save-KeePassPackage 
    $package.Dispose()
    $sourcePackage.Dispose();
    Write-Host "$GroupName in $fileName synced to $packageName"
}