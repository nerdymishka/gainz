

function Write-CmdletName() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    $name = $Model.Name;

    if($name) {
        if([String]::IsNullOrWhiteSpace($Template)){
            $Template = "`n# {0}`n"
        }

        return [String]::Format($Template, (Write-EscapedMarkdownString $name))
    }

    return
}