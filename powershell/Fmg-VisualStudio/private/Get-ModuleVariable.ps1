$fmgVisualStudio = New-Object PSCustomObject @{
    VsVersions = @{} 
    MsBuildPaths = @{}
    DefaultVsVersion = $null 
    VsPaths = $null 
    VsTestPaths = @{}
    VsLatestVersion = $null 
}

function Get-ModuleVariable() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name 
    )

    if(![string]::IsNullOrWhiteSpace($Name)) {
        return $fmgVisualStudio.$Name 
    }

    return $fmgVisualStudio
}