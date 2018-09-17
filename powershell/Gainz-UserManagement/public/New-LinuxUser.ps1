

function New-LinuxUser() {
    Param(
        [Parameter()]
        [String] $Name,

        [int] $Id = -1,

        [Switch] $NoPassword,
        
        [SecureString] $Password,
        
        [String] $FullName,
        
        [DateTime] $AccountExpiresOn = [DateTime]::MinValue,
        
        [int] $PasswordExpiresIn = -1,
        
        [String[]] $Groups,
        
        [int] $LoginRetries = 10
    )

    $splat = @("-m");

    $noninteractive = Test-NonInteractive 

    if([string]::IsNullOrWhiteSpace($Name)) {
      
        if($noninteractive) {
            throw [System.ArgumentNullException] "-Name"
            return;
        }

        While([string]::IsNullOrWhiteSpace($name)) {
            $name =Read-Host -Prompt "Name"
        }
    }

    if($NoPassword.ToBool() -eq $false -and !$Password -or $Password.Length -lt 8) {
        if($noninteractive) {
            if($null -eq $Password) {
                throw [System.ArgumentNullException] "-Password"
                return;
            }
            
            if($Password.Length -lt 8) {
                throw [Exception] "Password must be longer than 7 characters";
            }
        }

        While($null -eq $Password) {
            $Password = Read-Host -Prompt "Password" -AsSecureString 
            if($Password.Length -lt 8) {
                $Password = $Null;
                Write-Warning "Password must be longer than 7 characters";
            }
        }
    }

    if($Password) {
        $hasOpenSsl = Get-Command openssl -EA SilentlyContinue
        if(!$hasOpenSsl) {
            Write-Error "openssl is not found on the path. try installing the openssl package";
            return;
        }
        $pw = New-Object pscredential -ArgumentList "", $Password 
        $pw = $pw.GetNetworkCredential().Password
        $splat += "-p"
        $splat += $(echo $pw | openssl passwd -1 -stdin)
    }
  
    if(![string]::IsNullOrWhiteSpace($FullName)) {
        $splat += "-c"
        $splat += "`"$FullName`""
    }
    $now = [DateTime]::Now

    if($PasswordExpiresIn -gt -1) {
        $splat += "-K"
        $splat += "PASS_MAX_DAYS=$PassworExpiresIn"
    }

    if($LoginRetries -gt 0) {
        $splat +="-K"
        $splat += "LOGIN_RETRIES=$LoginRetries"
    }

    if($AccountExpiresOn -gt $now) {
        $splat += "-e"
        $splat += $AccountExpiresOn.ToString("yyyy-MM-dd")
    }

    if($Id -gt -1) {
        $splat += "-u"
        $splat += "$Id"
    }

    if($Groups -and $groups.Length -gt 0) {
        $splat += "-g"
        $splat += [string]::Join(",", $groups);
    }

    useradd $splat $Name

    $result = $LASTEXITCODE -eq 0


    

    if($result) {
        if($ID -eq -1) {
            $id = id -u $Name 
        }

        return [PSCustomObject]@{
            Name = $Name
            FullName = $FullName
            Id = $id 
        }
    }

    return $null;
}