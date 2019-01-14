function Read-GzWinChromeExtensions() {

<#
.SYNOPSIS
    Gets a list of chrome extensions from disk.

.DESCRIPTION
    Gets a list of chrome extensions from disk. If -All is 
    present, all user profiles are enumerated. If not present
    only the current user's directory is enumerated.

.PARAMETER All
    Instructs the function to enumerate all user profiles.

.EXAMPLE
    PS C:\> $extensions = Read-GzWinChromeExtensions
    returns an array of extensions and their locations and versions.

.INPUTS
    None

.OUTPUTS
    An array of PsCustomObjects with the following properties:
        name = [string] 
        appid = [string] id
        version = [string] version info
        dir = [string] directory for the extension
        user = [string] name of user directory 
#>

    [CmdletBinding()]
    Param(
        [Switch] $All
    )

    PROCESS {
        $set = @();

        if($all)
        {
            $elevated = Test-GzCurrentUserIsElevated;
            if(!$elevated)
            {
                Write-Warning "Pulling chrome extensions for all users requires admin rights."
                return
            }

            $drive = $env:SystemDrive
            if($null -eq $drive) {
                $drive = "C:"
            }
        
        
            $profiles = Get-ChildItem "$drive\Users"    
        } else {
            $profiles = @($Env:USERPROFILE)
        }

    
        foreach($profile in $profiles)
        {
            $extensions = Get-ChildItem "$($profile.FullName)\AppData\Local\Google\Chrome\User Data\Default\Extensions" -EA SilentlyContinue
            
            foreach($ext in $extensions)
            {
                $versionDirs = Get-ChildItem -Path $($ext.FullName)
                foreach($versionDir in $versionDirs)
                {
                    $appid = $ext.BaseName

                    $name = ""
                    if( (Test-Path -Path "$($versionDir.FullName)\manifest.json") ) {
                        try {
                            $json = Get-Content -Raw -Path "$($versionDir.FullName)\manifest.json" | ConvertFrom-Json
                        
                            $name = $json.name
                        } catch {
                            #$_
                            $name = ""
                        }
                    }

        
                    if( $name -like "*MSG*" ) {
                
                        if( Test-Path -Path "$($versionDir.FullName)\_locales\en\messages.json" ) {
                            try { 
                                $json = Get-Content -Raw -Path "$($versionDir.FullName)\_locales\en\messages.json" | ConvertFrom-Json
                                $name = $json.appName.message
                    
                                if(!$name) {
                                    $name = $json.extName.message
                                }
                                if(!$name) {
                                    $name = $json.extensionName.message
                                }
                                if(!$name) {
                                    $name = $json.app_name.message
                                }
                                if(!$name) {
                                    $name = $json.application_title.message
                                }
                            } catch { 
                                #$_
                                $name = ""
                            }
                        }
                    

                        if( Test-Path -Path "$($versionDir.FullName)\_locales\en_US\messages.json" ) {
                            try {
                                $json = Get-Content -Raw -Path "$($versionDir.FullName)\_locales\en_US\messages.json" | ConvertFrom-Json
                                $name = $json.appName.message
                                ##: Try a lot of different ways to get the name
                                if(!$name) {
                                    $name = $json.extName.message
                                }
                                if(!$name) {
                                    $name = $json.extensionName.message
                                }
                                if(!$name) {
                                    $name = $json.app_name.message
                                }
                                if(!$name) {
                                    $name = $json.application_title.message
                                }
                            } catch {
                                #$_
                                $name = ""
                            }
                        }

                    }

                    $now = [DateTime]::UtcNow;
                    $epoch = ($now.Ticks - 621355968000000000) / 10000;

                    
                    $record = [PSCustomObject] @{
                        name = $name 
                        appid = $appid
                        version = $versionDir.Name 
                        dir = $versionDir.FullName
                        user = $profile.Name 
                        rowCreatedAt = $epoch 
                        rowUpdatedAt = $epoch
                        rowRemovedAt = $null 
                        rowCreatedAtDisplay = $now.ToString()
                        rowUpdatedAtDisplay = $now.ToString()
                        rowRemovedAtDisplay = $null
                    }

                    $set += $record
                }
            }
        }

        return $set 
    }
}
