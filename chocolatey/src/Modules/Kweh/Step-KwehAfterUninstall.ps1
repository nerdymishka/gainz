function Step-KwehAFterUninstall() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Config,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context 
    )

    Write-Debug "After Uninstall Not Implemented"
}

Set-Alias -Name Step-AfterUninstall -Value Step-KwehAfterUninstall  