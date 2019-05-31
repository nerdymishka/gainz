
function Write-CmdletSynopsis() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Model,

        [Parameter(Position = 1)]
        [String] $Template
    )

    
    #Syntax & Synopsis are confusing
    $synopsis = $Model.Synopsis
    $name = $Model.Name.Trim()
    if($synopsis -is [string]) {
        if([String]::IsNullOrWhiteSpace($Template)){
            $Template = "`n## Synopsis`n`n{0}`n"
        }
        $synopsis = ($synopsis | Out-String).Trim() 
        
        return [String]::Format($Template, (Write-EscapedMarkdownString $synopsis))
    }

    return "";
}