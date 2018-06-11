function Set-KwehEnvironmentVariable() {
    Param(
        [String] $Name,
        [String] $Value,
        [ValidateSet("Machine", "User", "Process")]
        [String] $Scope = "Process"
    )

    [Environment]::SetEnvironmentVariable($Name, $Value, $Scope)
    Set-Item "Env:$Name" -Value $Value 
}

Set-Alias -Name Set-EnvironmentVariable -Value Set-KwehEnvironmentVariable 