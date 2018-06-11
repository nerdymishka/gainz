$kweh7z = $null
$compressedSingle = @("tbz", "tgz")
$tarCompressed = @("bz", "bz2", "7z", "gz", "xy")

function Expand-7zipArchive() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1)]
        [String] $Destination,

        [Switch] $Force,

        [Switch] $Rename,

        [securestring] $Password,

        [String] $Log,

        [String] $Include
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path 
    }

    $Path = (Resolve-Path $Path).Path 
    
    $file = Split-Path $Path -Leaf 

    $ext = $file.Substring($file.LastIndexOf(".") + 1)
    
    
    if($file -match "\.tar\." -and $tarCompressed.Contains($ext) -or $compressedSingle.Contains($ext)) {
        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($Path)
        $tmp = "$Env:Temp/$fileName"
        Expand-7zipArchive $Path "$tmp" -Force -Password $Password -Log $Log -Include $Include

        $Path = (Get-Item "$tmp/*.*").FullName
        if($Path -is [Array]) {
            Write-Warning "Path Found more than one file"
            $Path = $Path[0]
        }
    }

    $targetName = $null

    if($Rename.ToBool()) {
        $targetName = Split-Path $Destination -Leaf
        $Destination = Split-Path $Destination
    }

    if(!$kweh7z) {
        $kweh7z = (Get-Command 7z.exe -ErrorAction SilentlyContinue)
        
        if($kweh7z) {
            $kweh7z = $kweh7z.Path 
        } else {
            if($Env:ChocolateyInstall) {
                $kweh7z = "$Env:ChocolateyInstall\tools\7z.exe"
            } else {
                Write-Error "Please install 7zip on the path"
                return $false;
            }
        }
    }

    $args = @(
        "x",
        "`"$Path`"",
        "-o`"$Destination`""
    )
    
    if($Force.ToBool()) {
        $args += "-aoa"
    }

    if($Include) {
        $args += "$Include"
    }

    if($Password) {
        $argz = $args.CopyTo($argz, 0)
        $pw = ConvertFrom-SecureString $Password 
        $args += "-p`"$pw`""
        $argz += "-p`"****************`""
       
        Write-Debug "$kweh7z $([string]::Join(" ", $argz))"
    } else {
        Write-Debug "$kweh7z $([string]::Join(" ", $args))"
    }

    if($Force -and (Test-Path $Destination)) {
        Remove-Item $Destination -Recurse -Force | Write-Debug
    }
    
    if($Log) {
        & $kweh7z @args | Out-File -FilePath $Log -Append -Encoding "utf8"
    } else {
        & $kweh7z @args | Write-Debug
    }

    $result = $LASTEXITCODE -eq 0;
    
    if($Rename.ToBool()) {
        Write-Debug "Renaming folder to $Destination/$targetName "
        $fileName = [System.IO.Path]::GetFileNameWithoutExtension($Path)
        if(Test-Path "$Destination/$fileName") {
            Rename-Item "$Destination/$fileName" "$Destination/$targetName"
        } else {
            Write-Warning "Could not locate path to rename: $Destination/$fileName" 
        }
    }

    if($tmp) {
        Remove-Item $tmp -Filter -Recurse | Write-Debug
    }

    return $result;
}