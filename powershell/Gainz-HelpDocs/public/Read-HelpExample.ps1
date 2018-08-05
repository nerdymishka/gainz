
function Read-HelpExample() {
    Param(
        [PsCustomObject] $Example
    )

    $code = $Example.Code 
    $remarks = $Example.Remarks 

    $nextCode = $code.TrimEnd("`n")
    
    $nextRemark = "";
    if($remarks) {

        # Powershell only allows one line examples.
        
        # PS C:\> can be used to extend example code blocks
        # where "PS C:\>" acts as a marker to instruct
        # the code below to be added to the code example 

        foreach($line in $remarks.Split("`n")) {
            if($line.Trim().StartsWith("PS C:\>")) {
                
                $l = $line.SubString(7)
                $nextCode += "`n$l"
            } else {
                $nextRemark += "`n$line"
            }
        }
    }
    
    $Example.Code = (Write-EscapedMarkdownString $nextCode)
    $Example.Remark = (Write-EscapedMarkdownString $nextRemark)
	return $Example
}