

function New-KwehHardLink() {
    [CmdletBinding(SupportsShouldProcess = $True)]
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Path,


        [Alias("Link")]
        [Parameter(Position = 1, Mandatory = $true)]
        [String] $Destination 
    )

   
    $isDir = (Test-Path $Path -PathType "Container")
    if($isDir) {
        if( $PSVersionTable.PSVersion.Major -gt 4) {
          
            if($PSCmdlet.ShouldProcess("Junction", "$Destination => $Path")) {
                New-Item $Destination -ItemType Junction -Value $Path   | Write-Debug
                 
            }
           
            
        } else {
            $Path = $path.Replace("/", "\")
            $Destination = $Destination.Replace("/", "\")
            
            if($PSCmdlet.ShouldProcess("cmd /c mklink /J", "$Destination => $Path")) {
               
                cmd /c mklink /J "$Destination" "$Path" | Write-Debug
                
            } 
            
    
        }
     
    }
    


    if( $PSVersionTable.PSVersion.Major -gt 4) {
        if($PSCmdlet.ShouldProcess("HardLink", "$Destination => $Path")) {
            New-Item $Destination -ItemType HardLink -Value $Path  | Write-Debug
            
        }

        
    } else {
        $Path = $path.Replace("/", "\")
        $Destination = $Destination.Replace("/", "\")
        Write-Debug "mklink /H $Destination $Path"
        
        if($PSCmdlet.ShouldProcess("cmd /c mklink /J", "$Destination => $Path")) {
            cmd /c mklink /H "$Destination" "$Path"  | Write-Debug
           
        } 

        
    }
}

Set-Alias -Name New-HardLink -Value New-KwehHardLink