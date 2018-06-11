function Merge-KwehConfig() {
    Param(
        [Parameter(Position = 0)]
        [PSCustomObject] $Source,

        [Parameter(Position = 1)]
        [PSCustomObject] $Destination
    )

    $left = $Source
    $right = $Destination
    
    if($left -eq $null -or $right -eq $null) {
        return $Destination
    }
    
    $left | Get-Member -MemberType NoteProperty  | ForEach-Object {
        $name = $_.Name 
      
        $leftValue = $left.$name
        $rightValue = $right.$($name);
        
        if($rightValue -eq $null -and $leftValue -is [System.Management.Automation.PSCustomObject]) {
             $rightValue = New-Object PSObject 
             $rightValue = Merge-KwehConfig -Source $leftValue -Destination $rightValue
             $right | Add-Member -Type NoteProperty -Name $name -Value $rightValue
                
        } else {
            if($rightValue -ne $null) {
                if($rightValue -is [System.Management.Automation.PSCustomObject] -and $leftValue -is [System.Management.Automation.PSCustomObject]) {
                    $rightValue = Merge-KwehConfig -Source $leftValue -Destination $rightValue
                    $right.$name = $rightValue
                    return
                } else {
                    $right.$name = $leftValue;
                }
            
            } else {
                $right | Add-Member -Type NoteProperty -Name $name -Value ($leftValue)
            }   
        }
    }
    
    return $right;
}