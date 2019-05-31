
function Write-CmdletDescription() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    $description =$Model.Description;
    if($description) {
        $description = $description.Text;
    }

    if($description) {
        if([string]::IsNullOrWhitespace($Template)) {
            $Template = "`n## Description`n`n{0}`n"
        }

        return [string]::Format($Template, $description);
    }

    return "";
}