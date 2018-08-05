$escapeValues = @{
    '\' = '\\'
    '`' = '\`'
    '*' = '\*'
    '_' = '\_'
    '{' = '\{'
    '}' = '\}'
    '[' = '\['
    ']' = '\]'
    '(' = '\('
    ')' = '\)'
    '#' = '\#'
    '+' = '\+'
    '!' = '\!'
    '.' = '\.'
}

function Write-EscapedMarkdownString() {

    Param(
        [Parameter(Position = 0)]
        [String] $Value 
    )
    
    if([string]::IsNullOrWhiteSpace($Value)) {
        return '';
    }
    
    
    foreach($test in $escapeValues.Keys) {
        $Value = $Value.Replace($test, $escapeValues[$test])
    }
    

    $lines = $Value -Split "`n"
    $sb = New-Object System.Text.StringBuilder
    for($i =0 ; $i -lt $lines.Length; $i++) {
        $sb.Append($lines[$i].Trim()) | Out-Null 
        $sb.Append("`n") | Out-Null 
    }
    
    return $sb.ToString().Trim("`n")
}