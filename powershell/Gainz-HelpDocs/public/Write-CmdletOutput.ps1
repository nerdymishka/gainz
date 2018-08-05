
function Write-CmdletOutput() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    $outputTypes = Write-EscapedMarkdownString ($Model.ReturnValues | Out-String)
    if($outputTypes -and $outputTypes.Length -and !$outputTypes.Trim().StartsWith("returnValue")) {
        if([string]::IsNullOrWhitespace($Template)) {
            $Template += "`n## Outputs`n`n{0}`n`n"
        }

        return [String]::Format($Template, $outputTypes)
    }

    return ""
}