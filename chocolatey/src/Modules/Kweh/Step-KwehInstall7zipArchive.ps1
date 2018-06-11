function Step-KwehInstall7ZipArchive() {
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

    $params = @{
        Rename = if($Instructions.Rename) { $true } else { $false } 
        Force = $true
        Password = $Instructions.Password 
        Log = $Instructions.Log 
    }

    Write-Debug "RENAME $($params.Rename)"

    if(!$Instructions.extractDir) {
        
        Expand-7zipArchive ($Context.Package) -Destination $destination @params | Write-Debug
        return $true
    } else {
        $extractDir = $Instructions.extractDir;
        if($extractDir.StartsWith("./")) {
            $extractDir = $extractDir.Substring(2)    
        }
        $extractDir = ($Context | Resolve-StringTEmplate ($extractDir))
        $params.Add("Include", "$extractDir")
        
        Expand-7zipArchive ($Context.Package) -Destination $destination @params 
    
        return $true
    }
}

Set-Alias -Name Set-Install7ZipArchive -Value Step-KwehInstall7ZipArchive