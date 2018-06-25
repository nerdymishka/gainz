function Write-CmdletLink() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $HeaderTemplate,

        [Parameter(Position = 2)]
        [String] $ItemTemplate
    )

    $links = $Model.Links 
    if($links -and $links.Length) {
        if([String]::IsNullOrWhitespace($HeaderTemplate)) {
            $HeaderTemplate = @"
## Links

"@

        }

        if([String]::IsNullOrWhitespace($ItemTemplate)) {
            $ItemTemplate = @"
- [{0}]({1})
"@
        }

        $out = ""
        $out+= $HeaderTemplate;

        $model.Links | ForEach-Object {
            if($_ -eq $Null) {
                return;
            }
            $name = $_.Name 
            $link = $_.Link

            $out += [String]::Format($ItemTemplate, $name, $link)
        }

        $out += "`n`n"
        
        return $out;
    }
   

    return "";
}