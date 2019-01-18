

function Merge-GzWinTelemetryObject() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject[]] $LeftStack,

        [Parameter(Position = 1)]
        [PsCustomObject[]] $RightStack,

        [String] $Id,

        [ScriptBlock] $Select 
    )

    if(!$Select -and [string]::IsNullOrWhiteSpace($Id)) {
        throw "There must be a way to uniquely select an object"
    }

    if(![string]::IsNullOrWhiteSpace($Id)) {
       $Select = {
           Param(
               [PsCustomObject] $Src,

               [PsCustomObject] $Dest
           )

           if($src.$Id -eq $dest.$Id) {
               return $true;
           }

           return $false;
       }
    }

    $set = @()

    foreach($src in $LeftStack)
    {
        $found = $false;
        foreach($dest in $RightStack)
        {
            $match = & $Select -Src $src -Dest $Dest 
            if($match)
            {
                $found = $true;
                $updated = $false;
                $skip = @(
                    "rowUpdatedAt", 
                    "rowCreatedAt", 
                    "rowRemovedAt",
                    "rowCreatedAtDisplay",
                    "rowUpdatedAtDisplay",
                    "rowRemovedAtDisplay"
                )
                $src | Get-Member -MemberType NoteProperty | ForEach-Object {
                    $name = $_.Name
                    

                    if($skip.Contains($name)) {
                       return; 
                    }

                    $srcValue = $src.$Name 
                    $destValue = $dest.$Name  
                    if($srcValue -ne $destValue)
                    {
                        $updated = $true;
                        $dest | Add-Member -MemberType NoteProperty -Name $name -Value $value;
                    }
                }

                if($updated)
                {
                    $dest.rowUpdatedAt = $src.rowUpdatedAt 
                    $dest.rowUpdatedAtDisplay = $src.rowUpdatedAtDisplay 
                }

                $set += $dest;

                break;
            }
        }

        if(!$found)
        {
            $set += $src;
        }
    }

    $now = [DateTime]::UtcNow
    $epoch = ($now.Ticks - 621355968000000000) / 10000;

    foreach($item in $set)
    {
        $found = $false;
        foreach($src in $LeftStack)
        {
            $match = & $Select -Src $dest -Dest $src 
            if($match)
            {
                $item.rowRemovedAt = $null
                $item.rowRemovedAtDisplay = $null
                $found = $true;
                break;
            }
        }

        if(!$found)
        {
            $item.rowRemovedAt = $epoch
            $item.rowRemovedAtDisplay = $now.ToString(); 
        }
    }
}

function Merge-GzWin32App() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject[]] $LeftStack,

        [Parameter(Position = 1)]
        [PsCustomObject[]] $RightStack
    )
    return Merge-GzAnalyticsObject -LeftStack $LeftStack -RightStack $RightStack 
}