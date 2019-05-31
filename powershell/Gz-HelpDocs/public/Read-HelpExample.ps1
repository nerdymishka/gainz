
function Read-HelpExample() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Example
    )

   
    if(!$example) {
        $Example = [PSCustomObject]@{
            Code = ""
            Remarks = [PSCustomObject]@{
                Text = ""
            }
            IsEmpty = $true 
            Title = ""
        }
    } 
    $code = $Example.Code 
    $remarks = $Example.Remarks;
    if($remarks.Text) {
        $remarks = $remarks.Text;
    }

    if($code) {
        $nextCode = $code.TrimEnd("`n")
    } else {
        $nextCode = "";
    }
   
    
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
    
    $Example.Code =  $nextCode
    $Example.Remarks = Write-EscapedMarkdownString $nextRemark
	return $Example
}