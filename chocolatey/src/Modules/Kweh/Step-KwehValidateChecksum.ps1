

function Step-KwehValidateChecksum() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Path,

        [Parameter(Position = 1, Mandatory = $true)]
        [String] $Hash,

        [Parameter()]
        [String] $Algorithm = "SHA256"
    )

    $actual = Get-FileHash -Path $Path -Algorithm $Algorithm

    return $actual.Hash -eq $Hash
}

Set-Alias -Name Step-ValidateChecksum -Value Step-KwehValidateChecksum