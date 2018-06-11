function Step-KwehInstall() {
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
            "7zip" {
                write-Host $instructions 
                $Context | Step-KwehFetchInstaller -Instructions $Instructions
                $Context | Step-KwehInstall7zipArchive -Instructions $Instructions 
            }
            "zip" {
                Write-Host $Instructions.uri 
                $Context | Step-KwehFetchInstaller -Instructions $Instructions
                $Context | Step-KwehInstallZip -Instructions $Instructions 
            }
            "msi" {
                $Context | Step-KwehFetchInstaller -Instructions $Instructions
                $Context | Step-KwehInstallMicrosoftInstaller -Instructions $Instructions
            }
            "download" {
                $Context | Step-KwehFetchFile -Instructions $Instructions
            }
            default {
                throw [System.NotSupportedException] "Install Type $Name"
            }
        }
    }
}
Set-Alias -Name Step-Install -Value Step-KwehInstall