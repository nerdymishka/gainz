
<#
function Initialize-PowershellRcLocal {
    Param() {

    }

    $splat = "-NoLogo -NoProfile -NonInteractive -ExecutionPolicy ByPass"
    

    switch([Envionment]::OsVersion.Platform) {

        "Unix" {
            if(!(Test-Path "/etc/gainz/rc_local.ps1")) {
                New-Item "/etc/gainz/rc_local.ps1" -Value = "# Exit `nexit 0" -Force    
                chmod +x /etc/rc.local
            }

            $splat += "-File `"/etc/gainz/rc_local.ps1`""

            $systemd = "
[Unit]
Description=Gainz PowerShell RCLocal Script
ConditionPathExists=/etc/gainz/rc_local.ps1


[Service]
Type=forking
ExecStart=pwsh $splat
TimeoutSec=0
StandardOutput=tty
RemainAfterExit=yes
SysVStartPriority=99

[Install]
WantedBy=multi-user.target        
"
            $systemFile = "/etc/systemd/system/gainz-rc-local.service"
            if(!(Test-File $systemFile)) {
                $systemd | Out-File -FilePath $systemFile -Encoding "UTF8" 
            }


        }
        "Win32NT" {
            $WorkingDir = $WorkingDirectory
           
           
            if(!$AsUser.ToBool()) {
                $isElevated = Test-IsElevated;
                if(!$isElevated) {
                    $msg = "New-ScheduledPowershellTask requires admin rights "+
                    "if the -AsUser flag is not present"
                    Write-Error $msg  
                    return;
                }

                $user = New-ScheduledTaskPrincipal -GroupId "BUILTIN\Administrators" -RunLevel Highest
                if([String]::IsNullOrWhiteSpace($WorkingDirectory)) {
                    $WorkingDir = "$Env:ALLUSERSPROFILE\Gainz\Scripts"
                }
            } else {
                if([String]::IsNullOrWhiteSpace($WorkingDirectory)) {
                    $WorkingDir = "$Env:LOCALAPPDATA\Gainz\Scripts"
                }    
                $user = New-ScheduledTaskPrincipal -UserId (whoami) 
            }

            if(!(Test-Path $WorkingDir)) {
                New-Item $WorkingDirectory -ItemType Directory -Force  | Write-Debug
            }

            $both = [string]::IsNullOrWhiteSpace($Command) -and [String]::IsNullOrWhiteSpace($File)

            if($both) {
                Write-Error "-Command or -File must be be specified";
                return;
            }

            $task = Get-ScheduledTask -TaskName $Name -ErrorAction SilentlyContinue
            if ($null -ne $task) {
                if($Force.ToBool()) {
                    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false 
                } else {
                    Write-Error "Task $Name already exists. Use another name or use the -Force flag"
                    return;
                }
            }

            $splat = 
            $action = New-ScheduledTaskAction -Execute "powershell.exe" -Argument $splat -WorkingDirectory $WorkingDir 
            $trigger = New-ScheduledTaskTrigger -AtStartup 
            $task = New-ScheduledTask -Action $action -Principal $user -Trigger $trigger 
        }
    }
}
#>