
function Write-CmdletDoc() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [ScriptBlock] $Do 
    )

    if(!$Do) {
        $Do = {
            Param(
                [Parameter(Position = 0)]
                [PsCustomObject] $Model
            )

            $out = "";
            $out += Write-CmdletFrontMatter -Model $Model
            $out += Write-CmdletSynopsis $Model
            $out += Write-CmdletDescription $Model
            $out += Write-CmdletExample $model 
            $out += Write-CmdletSyntax $Model
            $out += Write-CmdletAlias $Model
            $out += Write-CmdletParameter $Model
            $out += Write-CmdletInput $Model
            $out += Write-CmdletOutput $Model 
            $out += Write-CmdletNote $model 
           
            $out += Write-CmdletLink $model 

            return $out;
        }
    }

    return & $Do -Model $Model 
}