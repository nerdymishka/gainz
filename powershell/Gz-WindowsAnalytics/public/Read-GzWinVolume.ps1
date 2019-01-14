function Read-GzWinVolume() {

<#
.SYNOPSIS
    Reads a list of windows volumes and returns an array of 
    information.

.DESCRIPTION
    Reads a list of windows volumes and returns an array of 
    information such as volume heath, size, size remaining,
    and volume name and drive letter.

.EXAMPLE
    PS C:\> $volumes = Read-GzWinVolume
    
.INPUTS
    None

.OUTPUTS
    The output is an array of PsCustomObjects with the following properties:
        drive = [string]
        healthStatus = [string] Ok|Bad
        operationalStatus = [string]
        path = [string] contains unique id
        size = [UInt64]
        type = [string] NTFS
        sizeDisplay = [string]
        sizeRemaining = [UInt64]
        sizeRemainingDisplay = [string]
        percentUser = [double]
        createdAt = [Int64] epoch
        updatedAt = [Int64] epoch
        createdAtDisplay = [string] datetime
        updatedAtDisplay = [string] datetime
.NOTES
    General notes
#>
    [CmdletBinding()]
    Param(

    )

    PROCESS {
        $set = Get-Volume
        $volumes = @();

        
        $now = [DateTime]::UtcNow;
        $epoch = ($now.Ticks - 621355968000000000) / 10000;

        foreach($v in $set) {
            if(![string]::IsNullOrWhiteSpace($v.DriveLetter)) {
                $volumes += [PsCustomObject]@{
                    drive = $v.DriveLetter 
                    healthStatus = $v.HealthStatus
                    operationalStatus = $v.operationalStatus
                    path = $v.Path
                    size = $v.Size 
                    type = $v.FileSystem
                    sizeDisplay = Format-GzFileSize -Length ($v.Size)
                    sizeRemaining = $v.SizeRemaining
                    sizeRemainingDisplay = Format-GzFileSize -Length ($v.SizeRemaining)
                    percentUser = $v.SizeRemaining / $v.Size
                    rowCreatedAt = $epoch 
                    rowUpdatedAt = $epoch
                    rowRemovedAt = $null 
                    rowCreatedAtDisplay = $now.ToString()
                    rowUpdatedAtDisplay = $now.ToString()
                    rowRemovedAtDisplay = $null
                }
            }
        }
    
        return $volumes
    }
}