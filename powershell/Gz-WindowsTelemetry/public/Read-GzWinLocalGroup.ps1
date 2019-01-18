

function Read-GzWinLocalGroup() {
    
    <#
    .SYNOPSIS
        Reads all the local users and returns meta information.
    
    .DESCRIPTION
        Reads all the loocal users and returns meta information such
        as password required, name, full name, enabled, sid, etc, All
        the dates are converted to epoch time
    
    .EXAMPLE
        PS C:\> <example usage>
        Explanation of what the example does
    .INPUTS
        None
    
    .OUTPUTS
        The output is an array of PsCustomObjects with the following properties:
    
            name: [string]
            displayName: [string]
            enabled: [boolea]
            description: [string]
            sid: [string]
            userMayChangePassword: [boolean]
            source: [string]
            passwordRequired: [boolean]
            passwordExpiresAt: [Int64?] epoch
            passwordChangeableAt: [Int64?] epoch
            passwordLastSetAt: [Int64?] epoch
            lastLogonAt: [Int64?] epoch
            accountExpiresAt: [Int64?] epoch
            passwordExpiresAtDisplay: [string] datetime
            passwordChangeableAtDisplay: [string] datetime
            passwordLastSetAtDisplay: [string] datetime
            lastLogonAtDisplay: [string] datetime
            accountExpiresAtDisplay: [string] datetime
            createdAt: [Int64?] epoch
            updatedAt: [Int64?] epoch
            removedAt: [Int64?] epoch
            createdAtDisplay: [string] datetime
            updatedAtDisplay: [string] datetime
            removedAtDisplay: [string] datetime
    
    .NOTES
        General notes
    #>
    
    [CmdletBinding()]
    Param(

    )

    PROCESS {
        $groups = Get-LocalGroup

        $set = @();
    

        foreach($g in $groups) {
            $now = [DateTime]::UtcNow
            $epoch = ($now.Ticks - 621355968000000000) / 10000;

            $set += [PsCustomObject]@{
                name = $g.Name
                description = $g.Description 
                sid = $g.SID.Value
                source = $g.PrincipalSource
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $null
            }
        }
        return $set
    }
}