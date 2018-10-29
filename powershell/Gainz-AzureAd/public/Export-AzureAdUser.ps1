

function Export-AzureAdUser() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Switch] $Csv 
    )

    if([string]::IsNullOrWhiteSpace($Path)) {
        
        $Path = "$HOME/gainz/azure-ad"
        if(!(Test-Path $Path)) {
            New-Item "$path" -ItemType Directory
        }

        $Path += "/aad-users.json"
    }

    # TODO: allow user defined mappings
    Test-AzureAdLoginStatus -Login
    $adUsers = Get-AzureADUser -All $true
    
    if((Test-Path $path)) {
        $users = Get-Content $path -Raw | ConvertFrom-Json
    } else {
        $users = @();
    }

    foreach($adUser in $adUsers) {
        $passwordForceChangeNextLogin = $null
        $passwordForceChangeIsEnforced = $null;

        if($adUser.PasswordPolicy) {
            $passwordForceChangeNextLogin = $adUser.ForceChangePasswordNextLogin
            $passwordForceChangeIsEnforced = $adUser.EnforceChangePasswordPolicy
        }

        $add = $true;

        foreach($u in $users) {
            
         
            
            if($u.id -eq $adUser.ObjectId) {
                $add = $false;
                
                if( $adUser.DisplayName.Contains(" Room")) {
                    $u.membershipStatus = "room"
                }

                if( $adUser.UserPrincipalName.Contains("#EXT#")) {
                    $u.membershipStatus = "guest"
                }

               
                $u.email = $adUser.UserPrincipalName
                $u.upn = $adUser.UserPrincipalName
                $u.displayName = $adUser.DisplayName
                $u.isEnabled = $adUser.AccountEnabled
                $u.aadUserType = $adUser.UserType 
                $u.givenName = $adUser.GivenName
                $u.surname = $adUser.Surname 
                $u.passwordForceChangeUpLogin = $passwordForceChangeNextLogin
                $u.passwordForceChangeIsEnforced = $passwordForceChangeIsEnforced
                $u.department = $adUser.Department 
                $u.jobTitle = $adUser.JobTitle

                $u | Add-Member -MemberType NoteProperty -Name 'phone' -Value $adUser.TelephoneNumber -Force
                $u | Add-Member -MemberType NoteProperty -Name 'cell' -Value $adUser.Mobile -Force
                $u | Add-Member -MemberType NoteProperty -Name 'office' -Value $adUser.PhysicalDeliveryOfficeName -Force
                $u | Add-Member -MemberType NoteProperty -Name 'company' -Value $adUser.CompmanyName -Force
                $u | Add-Member -MemberType NoteProperty -Name 'streetAddress' -Value $adUser.StreetAddress -Force
                $u | Add-Member -MemberType NoteProperty -Name 'locality' -Value $adUser.City -Force
                $u | Add-Member -MemberType NoteProperty -Name 'region' -Value $adUser.State -Force
                $u | Add-Member -MemberType NoteProperty -Name 'country' -Value $adUser.Country -Force
                $u | Add-Member -MemberType NoteProperty -Name 'postalCode' -Value $adUser.PostalCode -Force
              
                break;
            }
        }
        
        if(!$add) {
            continue;
        }


        $user = [PSCustomObject]@{
            aadObjectId = $adUser.ObjectId
            id = $adUser.ObjectId
            email = $adUser.UserPrincipalName
            upn = $adUser.UserPrincipalName
            displayName = $adUser.DisplayName 
            isEnabled = $adUser.AccountEnabled
            aadObjectType = $adUser.ObjectType 
            aadUserType = $adUser.UserType 
            givenName = $adUser.GivenName
            surname = $adUser.Surname 
            passwordForceChangeUpLogin = $passwordForceChangeNextLogin
            passwordForceChangeIsEnforced = $passwordForceChangeIsEnforced
            department = $adUser.Department
            jobTitle = $adUser.JobTitle
            membershipStatus = "Unknown"
            phone = $adUser.TelephoneNumber
            cell = $adUser.Mobile
            office = $adUser.PhysicalDeliveryOfficeName
            company = $adUser.CompmanyName
            streetAddress = $adUser.StreetAddress
            locality = $adUser.City
            region = $adUser.State 
            country = $adUser.Country
            postalCode = $adUser.PostalCode
        }

        $users += $user 
    }

    $users | ConvertTo-Json | Out-File -FilePath $Path -Encoding "UTF8" -Force

    if($Csv.ToBool()) {
        $csvPath = $Path.Replace(".json", ".csv")
        $users | ConvertTo-Csv -NoTypeInformation | Out-File $csvPath -Encoding "UTF8" -Force 
    }
}