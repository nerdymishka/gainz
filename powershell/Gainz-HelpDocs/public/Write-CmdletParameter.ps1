
function Write-CmdletParameter() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )


    $test = $Model.Parameters.Parameter;
    $parameters = $Model.Parameters;
   
    if($test -and $test.Length) {
        $out = "";
        if([String]::IsNullOrWhitespace($HeaderTemplate)) {
            $HeaderTemplate = "`n## Parameters`n`n"
        }


        $out += $HeaderTemplate;

        $parameters.Parameter | ForEach-Object {
            $description = $_.Description;
            if($description.Text) {
                $description = $description.Text;
                if($description -is [Array]) {
                    $description = [String]::Join("`n", $description)
                }
                
            }
            
            $name = (Write-EscapedMarkdownString $_.Name)
            $type = $_.type.name;
            $position = $_.Position
            $globbing = $_.Globbing
            $aliases = (Write-EscapedMarkdownString $_.Aliases)
            $description = (Write-EscapedMarkdownString $description)
            $required = (Write-EscapedMarkdownString $_.Required)
            $input = (Write-EscapedMarkdownString $_.PipelineInput)
            $default = (Write-EscapedMarkdownString $_.DefaultValue)
            
            if([string]::IsNullOrWhitespace($description)) {
                $description = "&nbsp;"
            }

            if([string]::IsNullOrWhitespace($input)) {
                $input = "&nbsp;"
            }

            if([string]::IsNullOrWhitespace($aliases)) {
                $aliases = "&nbsp;"
            }

            if([string]::IsNullOrWhitespace($default)) {
                $default = "&nbsp;"
            }

            $out += @"

### \-$name

$description

<table>
   <tbody>
        <tr>
            <td>Type:</td>
            <td>$type</td>
        </tr>
        <tr>
            <td>Aliases:</td>
            <td>$aliases</td>
        </tr>
        <tr>
            <td>Required:</td>
            <td>$required</td>
        </tr>
        <tr>
            <td>Position:</td>
            <td>$position</td>
        </tr>
        <tr>
            <td>Default:</td>
            <td>$default</td>
        </tr>
        <tr>
            <td>Accept Pipeline Input:</td>
            <td>$input</td>
        </tr>
        <tr>
            <td>Accept Wildcard Characters:</td>
            <td>$globbing</td>
        </tr>
   </tbody>
</table>


"@
        }

        

        return $out;
    }


    return "";
}