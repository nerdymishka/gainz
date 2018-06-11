function Step-KwehInstallMicrosoftInstaller() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Instructions,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context 
    )

    if($Context.OptInstall -or $Context.Extract) {
        $path = $context.Package
        $installLabel = $Context.InstallLabel
        $tmp =  $Context.Tmp 
        
        $dest = "$tmp/$installLabel/uncompressed"
        
        $exitCode = Expand-KwehMicrosoftInstaller -Path $path -Destination $dest 
       
        
        
        $extractDir = $Instructions.extractDir 
        $extractDir = Resolve-KwehPath "$extractDir" -BasePath $dest -Context $Context
        $extractDir = (Resolve-Path $extractDir).Path
        Write-Host "extractDir $extractDir"
        $destination = $context.Destination 
        Copy-Item "$extractDir/**" $destination -Force -Recurse
        return $true 
    } else {
        $dest = $Context.Destination 
        $args = $null

        if($dest) {
            $dest = $dest.Replace("/", "\")
            $args += " TARGETDIR=`"$dest`""
        }

        if($Context.UserInstall) {
            $args += " ALLUSERS=2 MSIINSTALLPERUSER"
        }

        if($Instructions.Args) {
            $args += " $($Instructions.Args)"
        }

        if($env:ChocolateyInstallArguments) {
            $args += " $env:ChocolateyInstallArguments"
        }

        $exitCode = Install-KwehMicrosoftInstaller -Path ($Context.Package) -ArgumentList $args 

        if($exitCode -eq 0) {
            return $true 
        }

        if(!$Instructions.ExitCodes) {
            return $false;
        }

        return $Instructions.ExitCodes.Contains($exitCode)
    }
}