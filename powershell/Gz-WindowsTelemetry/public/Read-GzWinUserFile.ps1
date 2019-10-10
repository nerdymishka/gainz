function Get-GzWinUserFile() {
    [CmdletBinding()]
    Param(
        [Switch] $All,

        [string[]] $Directories
    )
    
    PROCESS {
        $set = @();


        if($All)
        {
            $elevated = Test-GzCurrentUserIsElevated;
            if(!$elevated)
            {
                Write-Warning "Get a list of all user files requires admin rights."
                return $set;
            }

            $drive = $Env:SystemDrive 
            $patterns = @(
                "$drive\Users\**\OneDrive*",
                "$drive\Users\**\Documents",
                "$drive\Users\**\Photos",
                "$drive\Users\**\Videos",
                "$drive\Users\**\Downloads",
                "$drive\Users\**\Desktop",
                "$drive\Users\**\.ssh",
                "$drive\Users\**\.config",
                "$drive\Users\**\DropBox",
                "$drive\Users\**\Google\Drive",
                "$drive\Users\**\Creative Cloud Files"
            );
        
        } else {
            $patterns = @(
                "$Env:USERPROFILE\OneDrive*",
                "$Env:USERPROFILE\Documents",
                "$Env:USERPROFILE\Photos",
                "$Env:USERPROFILE\Videos",
                "$Env:USERPROFILE\Downloads",
                "$Env:USERPROFILE\Desktop",
                "$Env:USERPROFILE\.ssh",
                "$Env:USERPROFILE\.config",
                "$Env:USERPROFILE\DropBox",
                "$Env:USERPROFILE\Google\Drive",
                "$Env:USERPROFILE\Creative Cloud Files"
            )
        }

        if($Directories)
        {
            foreach($dir in $Directories) {
                $patterns += $dir;
            }
        }
    


        foreach($pattern in $patterns)
        {
            $dirs = Get-Item $pattern -EA SilentlyContinue -Force

            if(!$dirs) {
                continue;
            }

            if(!($dirs -is [Array]))
            {
                $dirs = @($dirs)
            }

            $names = [enum]::GetNames([System.IO.FileAttributes])
            $attr = @{};
            foreach($name in $names)
            {
                $enum = [System.IO.FileAttributes]$name
                $attr[$enum] = $name;
            }

            foreach($dir in $dirs)
            {
                $files = Get-ChildItem $dir.FullName -Recurse -EA SilentlyContinue -Force
                if(!$files)
                {
                    continue;
                }

                if(!($files -is [Array]))
                {
                    $files = @($files)
                }

                foreach($file in $files)
                {
                    $hash = $null;
                    if($file.Length -lt 200000000 -and !($file -is [System.IO.DirectoryInfo]))
                    {
                        $hash = Get-FileHash $file.FullName -Algorithm SHA256
                        $hash = $hash.Hash 
                    }

                    $accessedAt = ($file.LastAccessTimeUtc.Ticks - 621355968000000000) / 10000
                    $modifiedAt = ($file.LastWriteTimeUtc.Tisk - 621355968000000000) / 10000
                    $createdAt =  ($file.CreationTimeUtc.Ticks - 621355968000000000) / 10000
                    $fileAttributes = $file.Attributes 
                    if($fileAttributes -is [System.IO.FileAttributes]::Normal)
                    {
                        $attrs = @("Normal")
                    } else {
                        $attrs = @();
                        foreach($key in $attr.Keys)
                        {
                            if($fileAttributes.HasFlag($key)) {
                                $attrs[$attr[$key]]
                            }
                        }
                    }

                    $now = [DateTime]::UtcNow 
                    $set += [PsCustomObject]@{
                        path = $file.FullName
                        fileAccessedAt = $accessedAt
                        fileModifiedAt = $modifiedAt
                        fileCreatedAt = $createdAt
                        size = $file.Length
                        sizeDisplay = ($file | Format-GzFileSize)
                        hash = $hash
                        attributes = $file.Attributes.value__
                        attributeNames = $attrs
                        createdAt = $now 
                    }
                }
                
            }
        }

        return $set;
    }
}