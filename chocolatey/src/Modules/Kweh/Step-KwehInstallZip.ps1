function Step-KwehInstallZip() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Instructions,
        [Parameter(Position = 1, ValueFromPipeline = $True )]
        [PsCustomObject] $Context
    )

    if(!$Context.Package) {
        Write-Error "Context is missing downloaded package location"
        return $false 
    }

    $destination = $null 

    if($Context.Opt) {
        $opt = $Context.Opt
        $installLabel = $Context.InstallLabel 
        $destination = "$opt/$installLabel"
    }

    if($Context.Destination) {
        $destination = ($Context | Resolve-StringTemplate $Context.Destination)
    }

    if([string]::IsNullOrWhiteSpace($destination)) {
        Write-Error "Context requires either a destination path or values for ToolDir & PackageDir"
        return $false;
    }

    if(! (Test-PAth $destination)) {
        New-Item $destination -ItemType Directory -Force | Write-Debug
    }

    if(!$Context.Destination) {
        $Context | Add-Member -MemberType NoteProperty -Name "Destination" -Value $destination
    }

    if(!$Instructions.extractDir) {
        
        Expand-Archive ($Context.Package) -DestinationPath $destination | Write-Debug
        return $true
    } else {
        $tmp = $Context.Tmp
        $tmp = "$tmp/$installLabel/unzipped"
        Expand-Archive ($Context.Package) -DestinationPath $tmp
        $extractDir = $Instructions.extractDir;
        if($extractDir.StartsWith("./")) {
            $extractDir = $extractDir.Substring(2)    
        }
        $extractDir = ($Context | Resolve-StringTEmplate ($extractDir))
        $extractDir = "$tmp/$extractDir"
        Copy-Item "$extractDir/*" "$destination" -Recurse -Force | Write-Debug
    
        return $true
    }
}

Set-Alias -Name Set-InstallZip -Value Set-KwehInstallZip