

function Invoke-WindowsUpdateScript() {
    Param(
        [Parameter(Position = 0)]
        [Uri] $Uri,

        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [PsCredential] $Credential,

        [String] $RunName = "SolovisUpdateScript",

        [Switch] $RunOnReboot
    )

    if($Credential) {
        Enable-WindowsAutoLogin -Credential $Credential
    }
    $dir = "$($Env:ALLUSERSPROFILE)/solovis/updates"

    if(!$Uri.IsFile) {
        $localPath = $Uri.LocalPath;
        if($localPath) {
            $fileName = Split-Path $Uri.LocalPath -Leaf 
        } else {
            $fileName = [Guid]::NewGuid() + ".ps1"
        }

        if(!(Test-Path $dir)) {
            New-Item $dir -ItemType Directory -Force | Write-Debug
        }
        
        $script = "$dir/$fileName"
        Invoke-WebRequest -Uri $url -UseBasicParsing -OutFile $script 
    } else {
        $script = $Uri.LocalPath
    }



    if($Credential -or $RunOnReboot.ToBool()) {
        $wrapper = @"
Import-Module Solovis-WindowsAutomation -Force 
Invoke-WindowsUpdateScript -RunOnReboot -Script `"$Script`"
"@

        if(!(Test-Path $dir)) {
            New-Item $dir -ItemType Directory -Force | Write-Debug
        }

        $updateScript = "$dir/update-script.ps1"
        $wrapper | Out-File $updateScript -Encoding "UTF8";
       

        Add-WindowsAutoRunScript -Script $updateScript -Name $RunName -Force 
        $pref = $ErrorActionPreference
        $ErrorActionPreference = "Stop"
        try {

            & $Script 
        } catch {
            $ex = $_.Exception 
            $now = [DateTime]::UtcNow.ToString()
            $msg += "[error]:[$now]"
            $msg += " " + $ex.Message 
            $msg += "`n" + $ex.StackTrace 
            $msg >> "$dir/error"

            # stop rebooting if there is an error
            Remove-WindowsAutoRunScript -Name $RunName
            Disable-WindowsAutoLogin

            throw $ex
        } finally {
            $ErrorActionPreference = $pref  
        }
      

        if(!(Test-WindowsPendingReboot)) {
            Remove-WindowsAutoRunScript -Name $RunName
            Disable-WindowsAutoLogin
        } else {
            Invoke-WindowsReboot
        }

        return 
    }



    $pref = $ErrorActionPreference
    $ErrorActionPreference = "Stop"
    try {

        & $Script 
    } catch {
        $ex = $_.Exception 
        $now = [DateTime]::UtcNow.ToString()
        $msg += "[error]:[$now]"
        $msg += " " + $ex.Message 
        $msg += "`n" + $ex.StackTrace 
        $msg >> "$dir/error"

       

        throw $ex
    } finally {
        $ErrorActionPreference = $pref  
    }
}