
function Set-XFileOwner() {
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [ValidateNotNullOrEmpty()]
        [Parameter(ValueFromRemainingArguments = $true)]
        [String] $Path,

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String] $Owner,

        [String] $Group,

        [Alias("R")]
        [switch] $Recurse
    )

    PROCESS {
        $IsAdmin = Test-UserIsAdministrator 
        if(!$IsAdmin) {
            throw "Set-XFileOwner requires root or admin rights";
        }

        if($Path.EndsWith("*")) {
            throw "Path may not end with '*'"
        }
        $primary = Get-Item $Path -EA SilentlyContinue
        
       
        
        if(!$primary) {
            Write-Warning "Could not locate $Path"
            return;
        }

        if($Owner.Contains(":")) {
            $parts = $Owner.Split(":")
            $Owner = $parts[0]
            $Group = $parts[1]
        }
        
      

        $children = @();
        if($Recurse.ToBool()) {
           $children = Get-ChildItem $Path -Recurse -EA SilentlyContinue
        }

        $items = @();
        $items += $primary
        
        if($children)
        {
            if($children -is [Array]) {
                foreach($item in $children) {
                    $items += $item 
                }
            } else {
                $items += $children
            }
        }
        

        if(Test-OsPlatform "Mac", "Linux") {

            $cmd = "chown"
            $splat = @();
            if(![String]::IsNullOrWhiteSpace($Group)) {
                $splat += "${Owner}:${Group}"
            } else {
                $splat += $Owner
            }

            if($Recurse.ToBool()) {
                $splat += "-R"
            }

            $splat += "$Path"

            if($PSCmdlet.ShouldProcess("$cmd $([string]::Join(" ", $splat))")) {
                & $cmd @splat 
            }

        } else {
            $ntOwner = New-Object  System.Security.Principal.NTAccount($Owner)
            $ntGroup = $ntOwner
            $g = $Owner
            if(![string]::IsNullOrWhiteSpace($Group)) {
                if($Group -ne $Owner) {
                    $ntGroup =  New-Object  System.Security.Principal.NTAccount($Group)
                    $g = $Group;
                }
            }
            

            if($PSCmdlet.ShouldProcess("SetOwner($Owner),SetGroup($g) on $Path")) {
                foreach($item in $items) {
                        
                    $acl = Get-Acl $item.FullName
                    $acl.SetOwner($ntOwner)
                    $acl.SetGroup($ntGroup)  

                    Set-Acl $item.FullName -AclObject $acl 
                }
            }
        }
    }
}