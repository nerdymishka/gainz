

function Export-AzureAdDevices() {
    PAram(
        [String] $Path
    )
    $adDevices = Get-AzureADDevice -All:$true

    $devices = @();
    foreach($d in $adDevices) {
        

        $users = Get-AzureADDeviceRegisteredUser -ObjectId $d.ObjectID 
        if(!($users -is [Array])) { $users = @($users)}

        $u = "";
        foreach($user in $users) {
            $u += $user.UserPrincipalName + ";" 
        }

        $owners = Get-AzureADDeviceRegisteredOwner -ObjectId $d.ObjectID
        if(!($owners -is [Array])) { $owners = @($owners)}
        $o = "";
        foreach($owner in $owners) {
            $o += $owner.UserPrincipalName + ";"
        }

        
        $devices += [PSCustomObject]@{
            displayName = $d.displayName 
            users = $u
            owners = $o 
            lastLogon = $d.ApproximateLastLogonTimeStamp
            enabled = $d.AccountEnabled
            osName = $d.DeviceOSType 
            osVersion = $d.DeviceOSVErsion
            objectId = $d.objectId
            deviceId = $d.deviceId
            isManaged = $d.isManaged
            isCompliant = $d.isCompliant
        }
    }


    $devices | ConvertTo-Json -Depth 10 | Out-File "$Path" -Encoding "UTF8"
}

Export-AzureAdDevices ".\devices.json"

$data = Get-Content ".\devices.json" -Raw | ConvertFrom-Json
$data | ConvertTo-Csv -NoTypeInformation | Out-File ".\devices.csv" -Encoding "UTF8" 