function Invoke-WindowsReboot() {
    Param(
        [Parameter(Position = 0)]
        [Alias("Name")]
        [String] $RunKeyName = "SolovisDevOpsReboot",

        [Switch] $AutoRun,

        [Switch] $Force 
    )

    if($AutoRun.ToBool()) 
    {
        $Script = $MyInvocation.PSCommandPath

        Add-AutoRunScript -Script $Script -Force 
    }

    Restart-Computer -Force
}