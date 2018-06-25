
function Write-CmdletAlias() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $HeaderTemplate,

        [Parameter(Position = 2)]
        [String] $ItemTemplate
    )


    $aliases = $Model.Alias;
    if($aliases -and $aliases.Length) {
        $Name = $Model.Name;
        if([string]::IsNullOrWhitespace($HeaderTemplate)) {
            $HeaderTemplate = "`n## {0} Aliases`n`n"
        }

        if([string]::IsNullOrWhitespace($ItemTemplate)) {
            $ItemTemplate = "`n- {0}"
        }
        
        $out = [string]::Format($HeaderTemplate, $Name);
        
        $aliases | ForEach-Object {
            $name = $_.Name 
            $out += [string]::Format($ItemTemplate, $name)
        }
        $out + "`n"

        return $out 
    }

    return "";
}