
function Write-CmdletSyntax() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    $syntax = $Model.Syntax
    if($syntax -is [PsCustomObject]) {
        $syntax = ($syntax | Out-String).Trim()
    } else {
        $syntax = $Model.Synopsis
        $syntax = ($syntax | Out-String).Trim()
    }
    
    if($syntax -is [PsCustomObject]) {
        if([string]::IsNullOrWhitespace($Template)) {
            $Template = "`n## Syntax`n`n``````powershell`n{0}`n```````n"
        }
    
        return [string]::Format($Template, $syntax);
    }

    return "";
}