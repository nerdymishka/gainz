function Write-CmdletExample() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $HeaderTemplate,

        [Parameter(Position = 2)]
        [String] $ItemTemplate
    )

    $examples = $Model.Examples.examples
    if($examples -and $examples.Length) {
        if([String]::IsNullOrWhitespace($HeaderTemplate)) {
            $HeaderTemplate = "`n## Examples`n`n"

        }

        if([String]::IsNullOrWhitespace($ItemTemplate)) {
            $ItemTemplate = @"
### {0}

``````powershell
{1}
``````

{2}
"@

        }

        $out = ""
        $out += $HeaderTemplate;

        $examples.example | ForEach-Object {
            $model = Read-HelpExample -Model $_
            $code = $model.Code 
            $remarks = $model.Remarks
            $title = $model.Title 
            if($title) {
                $title = $model.Title.Trim("- ".ToCharArray())
            }
            

            $out += [String]::Format($ItemTemplate, $title, $code, $remarks)
        }

        return $out;
    }
   

    return "";
}