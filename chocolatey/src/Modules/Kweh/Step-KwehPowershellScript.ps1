function Step-KwehPowershellScript() {
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

    if($Context.OptInstall) {
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

    

    if(!$Context.Destination) {
        $Context | Add-Member -MemberType NoteProperty -Name "Destination" -Value $destination
    }

    $script = $Instructions.Script
    if(!$script) {
        throw [Exception] "The script section must have a powershell install script"
    }
    if($script.StartsWith("./")) {
        $script = $script.Substring(2)
        $toolsDir = $context.ToolsDir
        if(!$toolsDir) {
            $toolsDir = $context.PackageDir
        }

        $script = "$toolsDir/$script"
    }
    if(!(Test-Path $script)) {
        throw [System.IO.FileNotFoundException] $script 
    }

    return & $script -Instructions $Instructions -Context $Context
}

Set-Alias -Name Step-PowershellScript -Value Step-KwehPowershellScript