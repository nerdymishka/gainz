function Step-KwehUninstall() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Config,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context 
    )
    $Installers = $Config.Install;
    
    if(!$Installers) {
        Write-Debug "install section ommitted in config"
        return;
    }

    $skip = $false;
    $Installers | Get-Member -MemberType NoteProperty | ForEach-Object {
        if($skip) {
            return;
        }
        $Name = $_.Name 
        $Instructions = $Installers.$Name 
        switch($Name) {
            "zip" {
                $dest = $Context.Destination
                if(Test-Path $dest) {
                    Remove-Item "$dest" -Force -Recurse
                } 
            }
            "7zip" {
                $dest = $Context.Destination
                if(Test-Path $dest) {
                    Remove-Item "$dest" -Force -Recurse
                } 
            }
            "download" {
                $dest = $Context.Destination 
                if(Test-Path $dest) {
                    Remove-Item "$dest" -Force -Recurse
                }
            }
            "msi" {
                $Context | Step-KwehUninstallMicrosoftInstaller -Instructions $Instructions
            }
            default {
                throw [System.NotSupportedException] "Install Type $Name"
            }
        }
    }
}

Set-Alias -Name Step-Uninstall -Value Step-KwehUninstall 