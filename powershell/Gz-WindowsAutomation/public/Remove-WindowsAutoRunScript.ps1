function Remove-WindowsAutoRunScript() {
    Param(
        [Parameter(Position = 0)]
        [Alias("Name")]
        [String] $RunKeyName = "SolovisDevOpsReboot"
    )

    $runPath ="HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\run"
    $testKey = Get-ItemProperty -Path $runPath -Name $RunKeyName -EA SilentlyContinue
    if($testKey) {
        Remove-ItemProperty -Path $runPath -Name $RunKeyName | Write-Debug
    }
}