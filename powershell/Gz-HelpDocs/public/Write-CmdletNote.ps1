function Write-CmdletNote() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    $notes = (Write-EscapedMarkdownString ($Model.AlertSet | Out-String))
    if(![String]::IsNullOrWhitespace($notes)) {
        if([String]::IsNullOrWhitespace($Template)) {
            $Template = "`n## Note`n`n{0}`n`n"
        }

        return [String]::Format($Tempate, $notes);
    }

    return "";
}