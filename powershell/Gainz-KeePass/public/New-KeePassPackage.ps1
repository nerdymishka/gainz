function New-KeePassPackage() {
    <#
        .SYNOPSIS 
        Creates a new KeePass master key
    
        .DESCRIPTION
        The master key is a composite key that are required to open KeePass
        database files. Without it, the file can not be decrypted.
    
        .PARAMETER Path
        The file path to the KeePass database file
    
        .PARAMETER Key
        The KeePass composite master key, which is created by `New-KeePassKey`
    
        .PARAMETER Password
        (Optional) The password for the composite key.
    
        .PARAMETER KeyFile
        (Optional) A file of bytes for the composites key.
    
        .PARAMETER Do
        (Optional) A script block that binds `$_`, `$db`, and `$Package`
        to the block to allow you to interact with the KeePass database
        and then dispose it once the block finishes executing.
    
        .EXAMPLE
        $db = Open-KeePassPackage "$home/Desktop/passwords.kdbx" -Key $key
    
        .EXAMPLE
        PS C:\> $key | Open-KeePassPackage "$home/Desktop/passwords.kdbx" -Do {
        PS C:\>     $entry = $_ | Find-KeePassEntryByTitle "cert:azure"
        PS C:\>     $entry | Export-KeePassBinary -Name "azure.pfx" -DestinationPath "$home/Desktop/azure.pfx"
        PS C:\>     Write-Host ($entry.UnprotectPassword())
        PS C:\> }
    #>
        Param(
            [Parameter(Mandatory = $true, Position = 0)]
            [string] $Path,

            [string] $DatabaseName,

            [Parameter(ValueFromPipeline = $true)]
            [NerdyMishka.KeePass.MasterKey] $Key,
            [SecureString] $Password = $null,
            [string] $KeyFile = $null ,
            [switch] $UserAccount,
            [scriptblock] $Do
        )

        if([String]::IsNullOrWhiteSpace($Path)) {
            Write-Warning "-Path can not be null or empty";
            return $null;
        }

       
    
        $constructKey = ($Password `
                        -or ![string]::IsNullOrWhiteSpace($KeyFile) `
                        -or $UserAccount.ToBool())
    
        if($null -eq $key -and !$constructKey) {
            throw new ArgumentException("Key, Password, or KeyFile must have a value");
        }
    
        if($constructKey) {
            $Key = (New-KeePassKey -Password $Password -KeyFile $KeyFile -UserAccount:$UserAccount)
        }
        
        if([string]::IsNullOrWhiteSpace($DatabaseName)) {
            $DatabaseName = [IO.Path]::GetFileNameWithoutExtension($Path)
        }
        $package = New-Object NerdyMishka.KeePass.KeePassPackage
        $group = New-Object NerdyMishka.KeePass.KeePassGroup
        $group.Name = $DatabaseName 
        $package.Document.Add($group)
        $package.MetaInfo.DatabaseName = $DatabaseName
        $package.SetKey($key) | Out-Null
       
       
       

        if($Do -ne $null) {
            Set-Variable -Name "_" -Value $Package 
            $db = $Package
            #$underscore = New-Object PSVariable @("_", $Package)
            $vars = @(
                (Get-Variable -Name "db" -Scope 0),
                (Get-Variable -Name "Package"  -Scope 0),
                (Get-Variable -Name "_")
            )
            
            $Do.InvokeWithContext(@{}, $vars) | Out-Null
        
            return $Package | Save-KeePassPackage $Path -Key $Key 
        } else {
            return $Package | Save-KeePassPackage $Path -Key $key
        }
}