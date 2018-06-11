
$kwehUnits= @('B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB');

function Format-KwehFileSize() {
    Param (
        [Parameter(Mandatory=$true, Position=0)]
        [Double] $Bytes
    )

  
    # TODO force specific unit
    foreach ($unit in $kwehUnits) {
        if ($Bytes -lt 1024) {
            return [string]::Format("{0:0.##} {1}", $Bytes, $nit)
        }
        
        $Bytes /= 1024
    }

    return [string]::Format("{0:0.##} YB", $Bytes)
}

Set-Alias -Name Format-FileSize -Value Format-KwehFileSize 