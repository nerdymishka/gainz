
function Write-CmdletInput() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    $inputTypes = Write-EscapedMarkdownString ($Model.InputTypes | Out-String)
    if($inputTypes -and $inputTypes.Length -and !$inputTypes.Contains("inputType")) {
        if([string]::IsNullOrWhitespace($Template)) {
            $Template += "`n## Inputs`n`n{0}`n`n"
        }

        return [String]::Format($Template, $inputTypes)
    }

    return ""
}