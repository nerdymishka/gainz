function Write-CmdletExample() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $HeaderTemplate,

        [Parameter(Position = 2)]
        [String] $ItemTemplate
    )

    $examples = $Model.Examples
    if($examples) {
        $examples = $Model.Examples.example
        if(!($examples -is [Array])) {
            $examples = @($examples);
        }
    
    } else {
        return;
    }
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

        foreach($example in $examples) {
            $model = Read-HelpExample $example;
            if($model.IsEmpty) {
                return;
            }
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