# Gainz KeePass

A simple API for reading and writing values to a KeePass file.

### Install

```powershell
Install-Module Gainz-KeePass -Force
```

### Quick Start Example
```powershell
$pw = Read-Host -Prompt "Password?" -AsSecureString 
$defaultKdbx = "$Home/Desktop/Test.kdbx"

Open-KeePassPackage $defaultKdbx -Password ($pw) -Do {
    # $_ is a reference to the open package
    # Find-KeePassEntryByTitle will return one or more results for all
    # entries in the database that have the title that matches
    # Sample Entry 2
    $entry = $_ | Find-KeePassEntryByTitle "Sample Entry #2"
    if($entry -eq $Null) {
        Write-Warning "entry is null"
        return;
    }
    $pw = $entry.UnprotectPasswordAsSecureString();

    $pwString = $entry.UnprotectPassword();
}

$package = Open-KeePassPackage $defaultKdbx -Password $pw

# the path below is the root group "Test" and the name/title of the entry
# paths for this library are a combination of all the groups and the
# name/title of the entry.
$entry2 = $package | Find-KeePassEntry "Test/Sample Entry #2"
$entry3 = $package | Find-KeePassEntry "Test/SubGroup1/SubGroup2/Entry Title
$package.Dispose()

Write-Host ($entry2.Name) 
Write-Host ($entry2.Notes)
Write-Host ($entry2.Url)
Write-Host ($entry2.Tags)
Write-Host ($entry2.UnprotectPassword())
```

### Export a file
```powershell 
$test2Kdbx = "$Home/Desktop/Test2.kdbx"
$test2Key = "$Home/Desktop/Test2.key"
$dest = "$Home/Desktop/cert.pfx"

$package = Open-KeePassPackage $defaultKdbx -KeyFile  $test2Key
$entry = $package | Find-KeePassEntry "Test2/SubGroup1/MyCert" 
$entry | Export-KeePassBinary -Name "cert.pfx" -DestinationPath $dest
$package.Dispose()
```