function Get-KwehSemanticVersionRank() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [string[]]$Versions,

        [switch] $IsFileName 
    )
    
    $results = @()
    
    if($versions.Length -eq 1) {

        $current = New-Object PsObject
        $current | Add-Member -MemberType NoteProperty -Name Rank -Value 0
        $current | Add-Member -MemberType NoteProperty -Name Version -Value $Versions[0]
        $results += $current;

        return $results;
    }
    
    for($i = 0; $i -lt $Versions.Length; $i++) {
       
        $rank = 0;
        $version
        for($j = 0; $j -lt $Versions.Length; $j++) {
            $diff = 0;
            
            $expected = $Versions[$i];

            if($IsFileName.IsPresent) {

                $expected = [System.IO.Path]::GetFileName($expected)
                $first = $expected.IndexOf(".");
                $last = $expected.LastIndexOf(".")
                
                if($first -eq $last ) {
                    $length = 0;
                } else {
                     $length = ($last - $first) + -1;
                }
               
                if($length -eq 0) {
                    $expected = "0.0.0";
                } else {
                    $expected = $expected.Substring($first + 1, $length);
                }
            }
   
            $version = $Versions[$j];
 
            if($IsFileName.IsPresent) {

                $version = [System.IO.Path]::GetFileName($version)
                $first = $version.IndexOf(".");
                $last = $version.LastIndexOf(".")
                
                if($first -eq $last ) {
                    $length = 0;
                } else {
                     $length = ($last - $first) + -1;
                }
               
                if($length -eq 0) {
                    $version = "0.0.0";
                } else {
                    $version = $version.Substring($first + 1, $length);
                }
            }
          
            $diff = Compare-KwehSemanticVersion $expected $version
          
            if($diff -gt 0) {
                $rank++;
            }
        }

        $current = New-Object PsObject
        $current | Add-Member -MemberType NoteProperty -Name Rank -Value $rank
        $current | Add-Member -MemberType NoteProperty -Name Version -Value $Versions[$i]
        $current | Add-Member -MemberType NoteProperty -Name Index -Value $i
        $results += $current;
    }

    return $results;
}

Set-Alias -Name Get-SemanticVersionRank -Value Get-KwehSemanticVersionRank