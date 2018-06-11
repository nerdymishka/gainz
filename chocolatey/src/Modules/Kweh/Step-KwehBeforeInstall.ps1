function Step-KwehBeforeInstall() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Config,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context
    )

    if($Context.beforeInstall -and $Context.beforeInstall -is [Array]) {
        foreach($directive in $Context.beforeInstall) {
            $script = $Context | Resolve-StringTemplate $directive;
            if($script.StartsWith("./")) {
                $script = $script.Substring(2)
                $script = ($Context.PackageDir.Replace("\", "/")) + "/$script" 
            }
            & $script -Config $Config -Context $Context;
        }
    }
}

Set-Alias -Name Step-BeforeInstall -Value Step-KwehBeforeInstall