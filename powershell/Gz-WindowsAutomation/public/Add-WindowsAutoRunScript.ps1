function Add-WindowsAutoRunScript() {
    Param(
        [Parameter(Position = 0)]
        [Alias("Name")]
        [String] $RunKeyName = "SolovisDevOpsReboot",

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [String] $Script,

        [Switch] $Force 
    )

    if(!(Test-Path $Script)) {
        throw "FileNotFound $Script";
    }

    $runPath ="HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\run"
 
    #PowerShell PATH = C:\WINDOWS\system32\WindowsPowerShell\v1.0\powershell.exe
    $powershellExe = (Join-Path $env:windir "system32\WindowsPowerShell\v1.0\powershell.exe")
    $value = "$PowershellExe -ExecutionPolicy Bypass -NonInteractive -File `"$Script`" " 

    $testKey = Get-ItemProperty -Path $runPath -Name $RunKeyName -EA SilentlyContinue
    if($testKey) {
        if(!$Force.ToBool()) {
            throw "AutoStartScript Key Exists $RunKeyName"
        }

        Remove-ItemProperty -Path $runPath -Name $RunKeyName | Write-Debug
    }

    New-ItemProperty -Path $runPath -Name $RunKeyName -Value $value | Write-Debug
}