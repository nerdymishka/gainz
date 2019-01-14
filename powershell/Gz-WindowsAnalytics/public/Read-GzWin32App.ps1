

function Read-GzWin32App() {
<#
.SYNOPSIS
    Reads the registry for all Win32 applications that are installed.

.DESCRIPTION
    Reads the registry for all Win32 applications that are installed 
    including applications installed by other users. This command requires
    administrative rights unless the -CurrentUser flag is present.

.PARAMETER Machine
    Limits the results to applications that are only
    installed at the machine level. 

.PARAMETER CurrentUser
    Limits the results to applications that are only
    installed by the current user running this command.

.EXAMPLE
    PS C:\> $data = Read-GzWin32App
    Returns metadata about installed applications

.INPUTS
    Inputs (if any)
.OUTPUTS
    The output is an array of PsCustomObjects with the following properties:

        displayName: [string]
        keyName: [string]
        displayVersion: [string]
        installDate: [string]
        installLocation: [string]
        estimatedSize: [double]
        publisher: [string]
        uninstallString: [string]
        innoSetupVersion: [string] 
        helpLink: [string]
        x64: [boolean]
        scope: [string] machine|user
        createdAt = [Int64] epoch time 
        updatedAt = [Int64] epoch time
        createdAtDisplay: [string] datetime
        updatedAtDisplay: [string] datetime
        removedAt: [Int64?] epoc time, defaults to null 

.NOTES
    General notes
#>
    [CmdletBinding()]
    Param(
        [Switch] $CurrentUser,
        [Switch] $Machine
    )

    
    PROCESS {
        $all = !$CurrentUser.ToBool() -and !$Machine.ToBool()
        $keys = @();
        $users = @();
        $set = @();
        $elevated = Test-GzCurrentUserIsElevated;
    
        if($all -and $elevated)
        {
            $drive = $env:SystemDrive
            if($null -eq $drive) {
                $drive = "C:"
            }
        
        
            $profiles = Get-ChildItem "$drive\Users"
            Register-GzRegistryUserHive | Out-Null 
            foreach($p in $profiles)
            {
                if($p.Name -eq "Public")
                {
                    continue;
                }
    
                if($p.FullName -eq $env:USERPROFILE)
                {
                    continue;
                }
    
                $users += $p.Name;
                $null = reg load HKU\$($p.Name) "$($p.FullName)\NTUSER.DAT" 2>&1 | Out-Null
    
    
    
                $keys += "HKU:\$($p.Name)\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            }
        }
    
      
     
        if($all -or $CurrentUser.ToBool())
        {
            $keys += "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
            
        }
        if($all -or $Machine.ToBool())
        {
            if($elevated)
            {
                $keys += "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                $keys += "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";    
            } else {

                $msg = "This current for this session must be elevated to Administrator before "
                $msg += "this command can pull applications items from the machine level registry. "
                Write-Warning $msg
            }
        }
       
    
    
        try {
        
            foreach($key in $keys)
            {
                if($key.StartsWith("HKU") -and $registered)
                {   
                    $v = $key.SubString(5);
                    $subKey =  [Microsoft.Win32.Registry]::Users.OpenSubKey($v)
                
                    $children = @();
                    if($subKey)
                    {
                        $childrenNames = $subKey.GetSubKeyNames();
                        if($childrenNames)
                        {
                            foreach($childrenName in $childrenNames)
                            {
                                $children += $subKey.OpenSubKey("$childrenName")
                            }
                        }
                    }
                    
                    $subKey.Close() | Out-Null
                    $subKey.Dispose() | Out-Null
                
                } else {
                    $children = Get-ChildItem $key -ErrorAction SilentlyContinue
                }
    
                $scope = "user"
                $x64 = $true;
                if($key -match "HKLM") {
                    $scope = "machine";
                }
    
                if($key -match "WOW6432") {
                    $x64 = $false;
                }
    
                if(!$children) { $children = @() } 
                
    
                foreach($child in $children)
                {
                    $valueNames = $child.GetValueNames();
    
                    if($valueNames -and $valueNames.Length)
                    {
                        $now = [DateTime]::UtcNow;
                        $epoch = ($now.Ticks - 621355968000000000) / 10000;
                        
                        
                        $set += [PsCustomObject]@{
                            displayName = $child.GetValue("DisplayName")
                            keyName = $child | Split-Path -Leaf
                            displayVersion = $child.GetValue("DisplayVersion")
                            installDate = $child.GetValue("InstallDate")
                            installLocation = $child.GetValue("InstallLocation")
                            estimatedSize = $child.GetValue("EstimatedSize")
                            publisher = $child.GetValue("Publisher")
                            uninstallString = $child.GetValue("UninstallString")
                            innoSetupVersion = $child.GetValue("Inno Setup: Setup Version")
                            helpLink = $child.GetValue("HelpLink")
                            x64 = $x64
                            scope = $scope
                            rowCreatedAt = $epoch 
                            rowUpdatedAt = $epoch
                            rowRemovedAt = $null 
                            rowCreatedAtDisplay = $now.ToString()
                            rowUpdatedAtDisplay = $now.ToString()
                            rowRemovedAtDisplay = $null
                        }
                    }
    
                    # important for unloading the hive
                    $child.Close() | Out-Null
                    $child.Dispose() | Out-Null
                }
            }
        } finally {
            if($users.Length -gt 0)
            {
                foreach($user in $users)
                {
                    reg unload "HKU\$user" | Out-Null
                }
            }
            
        }
    
        return $set;
    }
}