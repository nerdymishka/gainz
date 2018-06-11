function Install-KwehInnoInstaller() {
    Param(
        [String] $Path, 
        [String] $Log,
        [String] $Destination = $null,
        [switch] $Force,
        [switch] $NoIcons,
        [string[]] $Components,
        [String[]] $MergeTasks,
        [String] $SilentArgs = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART"
    )

    if(![string]::IsNullOrWhiteSpace($Log)) {
        $SilentArgs += " /Log=`"$Log`""
    }

    if($Force.ToBool()) {
        $SilentArgs += " /FORCECLOSEAPPLICATIONS"
    }

    if(![string]::IsNullOrWhiteSpace($Destination)) {
        $SilentArgs += " /Dir=`"$Destination`""
    }

    if($MergeTasks -ne $Null -and $MergeTasks.Length -gt 0) {
        $SilentArgs += " /MERGETASKS=`"$([string]::Join(",", $MergeTasks))`""
    }

    if($Components -ne $Null -and $Components.Length -gt 0) {
        $SilentArgs += " /COMPONENTS=`"$([string]::Join(",", $Components))`""
    }

    if($NoIcons.ToBool()) {
        $SilentArgs += " /NOICONS"
    }

    $Path = (Resolve-Path $Path).Path

    & $PATH $SilentArgs
    Start-Sleep 1
    return $LASTEXITCODE 
}
Set-Alias -Name Install-InnoInstaller -Value Install-KwehInnoInstaller