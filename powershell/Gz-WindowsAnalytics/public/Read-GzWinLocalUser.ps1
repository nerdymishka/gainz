

function Read-GzWinLocalUser() {
    
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
        $users = Get-LocalUser

        $set = @();
    

        foreach($u in $users) {
            $passwordExpires = $u.PasswordExpires
            $passwordChangeable = $u.PasswordChangeableDate
            $passwordLastSet = $u.PasswordLastSet
            $lastLogon = $u.LastLogon
            $accountExpires = $u.accountExpires


            $passwordExpiresAt = $null
            $passwordChangeableAt = $null
            $passwordLastSetAt = $null
            $lastLogonAt = $null
            $accountExpiresAt = $null

            $passwordExpiresAtDisplay = $null
            $passwordChangeableAtDisplay = $null
            $passwordLastSetAtDisplay = $null
            $lastLogonAtDisplay = $null
            $accountExpiresAt = $null

            if($passwordExpires) {
                $passwordExpires = $passwordExpires.ToUniversalTime()
                $passwordExpiresAt = ($passwordExpires.Ticks - 621355968000000000) / 10000;
                $passwordExpiresAtDisplay = $passwordExpires.ToString()
            }

            if($passwordChangeable) {
                $passwordChangeable = $passwordChangeable.ToUniversalTime()
                $passwordChangeableAt = ($passwordChangeable.Ticks - 621355968000000000) / 10000;
                $passwordChangeableAtDisplay = $passwordChangeable.ToString()
            }

            if($passwordLastSet) {
                $passwordLastSet = $passwordLastSet.ToUniversalTime()
                $passwordLastSetAt = ($passwordLastSet.Ticks - 621355968000000000) / 10000;
                $passwordLastSetAtDisplay = $passwordLastSet.ToString()
            }

            if($lastLogon) {
                $lastLogon = $lastLogon.ToUniversalTime()
                $lastLogonAt = ($lastLogon.Ticks - 621355968000000000) / 10000;
                $lastLogonAtDisplay = $lastLogon.ToString()
            }

            if($accountExpires) {
                $accountExpires = $accountExpires.ToUniversalTime()
                $accountExpiresAt = ($accountExpires.Ticks - 621355968000000000) / 10000;
                $accountExpiresAtDisplay = $accountExpires.ToString()
            }


            $now = [DateTime]::UtcNow
            $epoch = ($now.Ticks - 621355968000000000) / 10000;

            $set += [PsCustomObject]@{
                name = $u.Name
                displayName = $u.FullName
                enabled = $u.Enabled
                description = $u.Description 
                sid = $u.SID
                userMayChangePassword = $u.UserMayChangePassword
                source = $u.PrincipalSource
                passwordRequired= $u.PasswordRequired
                passwordExpiresAt = $passwordExpiresAt
                passwordChangeableAt = $passwordChangeableAt
                passwordLastSetAt = $passwordLastSetAt
                lastLogonAt = $lastLogonAt 
                accountExpiresAt = $accountExpiresAt
                passwordExpiresAtDisplay = $passwordExpiresAtDisplay
                passwordChangeableAtDisplay = $passwordChangeableAtDisplay
                passwordLastSetAtDisplay = $passwordLastSetAtDisplay
                lastLogonAtDisplay = $lastLogonAtDisplay
                accountExpiresAtDisplay = $accountExpiresAtDisplay
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