

function Set-XFileMode() {
<#
.SYNOPSIS
    Short description
.DESCRIPTION
    Long description
.EXAMPLE
    PS C:\> <example usage>
    Explanation of what the example does

.PARAMETER Path
    The path to the file or directory that must be changed

.PARAMETER Umask
    The unix permission code such as 755 or 0644

.PARAMETER Clean
    Clean is a dangerous flag that will remove all other access control
    list data and only retain the user, group, and other permissions.
    
    On windows, "Everyone" is used for other, the owner is the user, and
    the primary group is the group.

.PARAMETER Force
    Force is used avoid the prompt when the -Clean flag is used.

.INPUTS
    Inputs (if any)
.OUTPUTS
    Output (if any)
.NOTES
    General notes
#>
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [ValidateNotNullOrEmpty()]
        [Parameter(ValueFromRemainingArguments = $true)]
        [String] $Path,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Mode, 

        [Alias("r")]
        [Switch] $Recurse,

        [Alias("c")]
        [Switch] $Clean,

        [Alias("f")]
        [Switch] $Force
    )

    PROCESS {
        if($Clean.ToBool())
        {
            if(!$Force.ToBool()) {
                $d = Read-Host -Prompt "Are you sure you want to remove other permissions? "
                if(!($d -eq "y" -or $d[0] -eq "y")) {
                    Write-Host "$d was chosen. Action Cancelled."
                    return 
                }
            }
        }
        if(Test-OsPlatform "Linux", "Mac") {
            $cmd = "chmod"
            $splat += "$Path"
            if($Recurse.ToBool()) {
                $splat += "-R"
            }
            $splat += $Code.ToString()

            if($PSCmdlet.ShouldProcess("$cmd $([string]::IsNullOrWhiteSpace($splat))")) {
                & $cmd @splat 
            }

            

            if($Clean.ToBool() -and $Force.ToBool()) {
                $noAcl = $null -eq (Get-Command setfacl -EA SilentlyContinue)
                if($noAcl)
                {
                    if($PSCmdlet.ShouldProcess("Write-Warning `"setfacl is not installed`""))
                    {
                        Write-Warning "setfacl is not installed"
                        return;
                    }
                }

                if($PSCmdlet.ShouldProcess("setfacl -D e $Path")) {
                    setfacl -D e $Path 
                }
                
            }
            
        } else {

            $parts =  $Mode
            if(![char]::IsDigit($Mode[0])) {
                throw "Set-XFileMode on windows currently only supports digit umasks"
            }

            if($Mode.Length -lt 3) {
                throw "Set-XFileMode on windows requires at least 3 digits"
            }

            if($Mode.Length -gt 4) {
                throw "Set-XFileMode on windows umask number cannot be greater than 4 digits"
            }

            if($parts.Length -gt 3) {
                $parts = $parts.Substring(1)
            }

            $u = $parts[0]
            $g = $parts[1]
            $e = $parts[2]

          
    
            $objects = @();

          
            $data = (Get-ACL $Path)
            foreach($d in $data.Access) {
                $objects += $d.IdentityReference.Value
            }

            $splat = @($Path)
    
            function Get-ChmodSplat() {
                Param(
                    [String] $Name,
    
                    [String] $Code 
                )
    
                $splat = @();
    
                switch($Code) {
                    "0" { 
                        $splat += "/remove:g"
                        $splat += $Name  
                    }
                    "1" {
                        $splat += "/grant"
                        $splat += "${Name}:(ge)"
                    }
                    "2" {
                        $splat += "/grant"
                        $splat += "${Name}:(gw)"
                    }
                    "3" {
                        $splat += "/grant"
                        $splat += "${NAME}:(gw,ge)"
                    }
                    "4" {
                        $splat += "/grant"
                        $splat += "${Name}:(r)" 
                    }
                    "5" {
                        $splat += "/grant"
                        $splat += "${Name}:(rx)" 
                    }
                    "6" {
                        $splat += "/grant"
                        $splat += "${Name}:(m)" 
                    }
                    "7" {
                        $splat += "/grant"
                        $splat += "${Name}:(f)" 
                    }
                }

                return $splat 
            }
    
            if($Clean.ToBool() -and $Force.ToBool()) {
                $filter = @($data.Owner, $data.Group, "BUILTIN\Users", "Everyone")
                
                $splat += "/inheritance:r"
               

                foreach($o in $objects) {
                  
                    if($filter.Contains($o)) {
                        continue;
                    }
    
                    $splat += "/remove:g"
                    $splat += $o
                }
            }
    
            # NOTES:  Group is the Posix compatible primary group
            # Its not really used by windows

            # Also Groups on windows tend not to be named the same 
            # as a user. So if the group is the same as the owner
            # we should take the highest permission
            
            if($data.Owner -eq $data.Group) {
                $o = $u;
                if($g -gt $u) {
                    $o = $g;
                }
    
                $next = Get-ChmodSplat -Name $data.Owner -Code $o 
                foreach($n in $next) {
                    $splat += $n
                }
                
            } else {
                $next = Get-ChmodSplat -Name $data.Owner -Code $u 
                foreach($n in $next) {
                    $splat += $n
                }
                
                $next = Get-ChmodSplat -Name $data.Group -Code $g
                foreach($n in $next) {
                    $splat += $n
                }
            }
    
    
            
            if($objects.Contains($users) -and $data.Group -ne $users -and $data.Owner -ne $users) {
                $next = Get-ChmodSplat -Name $users -Code $g
                foreach($n in $next) {
                    $splat += $n
                }
            }
            
            $removeEveryone = $objects.Contains("Everyone") -and $e -eq 0
            if($removeEveryone -or ($Clean.ToBool() -and $Force.ToBool())) {
                $next = Get-ChmodSplat -Name "Everyone" -Code $e
                foreach($n in $next) {
                    $splat += $n
                }
            }

            if($Recurse.ToBool()) {
                $splat += "/t"
            }

            $cmd = "icacls.exe"
    
            if($PSCmdlet.ShouldProcess("$cmd $([string]::Join(" ", $splat))")) {
                & $cmd @splat 
            }
        }
    }
}