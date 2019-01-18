function Get-GzWinLocalGroupMember() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Group 
    )
    

    Process {
        $users = net localgroup $Group 2>1;

        $start = $false;
        $set = @();
      
        foreach($line in $users)
        {
            
            if($line.StartsWith("---")) { 
                $start = $true;
                continue;
            } 
    
            if($line.StartsWith("The command")) { 
               
                $start = $false; 
                break; 
            }
    
            if($start) {
               
                $set += $line.Trim();
            }
    
            
        }
       
        return $set;
    }
    
}