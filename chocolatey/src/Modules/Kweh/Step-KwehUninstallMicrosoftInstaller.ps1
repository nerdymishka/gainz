function Step-KwehUninstallMicrosoftInstaller() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Instructions,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context 
    )

    if($Context.OptInstall -or $Context.Extract) {
        $destination = $Context.Destination 

        if(!$destination) {
            $opt = $Context.Opt
            $installLabel = $Context.installLabel 
            $destination = "$opt/$installLabel"
        }

        Remove-Item ($destination) -Force -Recurse

        return $true 
    } else {
       
        $displayName = $Instructions.DisplayName 
        $guid = $Instructions.Guid 
        $silentArgs = $Instructions.SilentArgs 

        $args = New-Object PsCustomObject 
        if($displayName) {
            $args | Add-Member NoteProperty -Name "DisplayName" -Value $displayName 
        }
        if($guid) {
            $args | Add-Member NoteProperty -Name "Guild" -Value $guid
        }
        if($silentArgs) {
            $args | Add-Member NoteProperty -Name "SilentArgs" -Value $silentArgs
        }
        

        $exitCode = Uninstall-KwehMicrosoftInstaller @args 

        if($exitCode -eq 0) {
            return $true 
        }

        if(!$Instructions.ExitCodes) {
            return $false;
        }

        return $Instructions.ExitCodes.Contains($exitCode)
    }
}