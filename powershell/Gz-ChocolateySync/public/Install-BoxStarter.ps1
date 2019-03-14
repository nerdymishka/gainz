

function Install-Boxstarter {
    Param(
        [string] $Version = $null,
        [switch] $Force
    )

    if(Test-Path "$Env:ProgramData\boxstarter") {
        Write-Debug "BoxStarter Install folder already exists"

        if(!$Force.ToBool()) {
            return;
        }
    }

    if($null -eq (Get-Command choco.exe -EA SilentlyContinue)) {
        if($Force.ToBool()) {
            Install-Chocolatey -Force
        } else {

            Write-Warning "Chocolatey is not installed, execute Install-Chocolatey first."
            return;
        }
    }

    if(!(Test-IsAdmin)) {
        $bootstrapperFile = ${function:Get-Boxstarter}.File
        if($bootstrapperFile) {
            Write-Host "User is not running with administrative rights. Attempting to elevate..."
            $command = "-ExecutionPolicy bypass -noexit -command . '$bootstrapperFile';Get-Boxstarter $($args)"
            Start-Process powershell -verb runas -argumentlist $command
        }
        else {
            Write-Warning "User is not running with administrative rights.`nPlease open a PowerShell console as administrator and try again."
        }
        return
    }

    $badPolicy = $false
    @("Restricted", "AllSigned") | ? { $_ -eq (Get-ExecutionPolicy).ToString() } | % {
        Write-Warning "Your current PowerShell Execution Policy is set to '$(Get-ExecutionPolicy)' and will prohibit boxstarter from operating propperly."
        Write-Warning "Please use Set-ExecutionPolicy to change the policy to RemoteSigned or Unrestricted."
        $badPolicy = $true
    }
    
    if($badPolicy) { return }

    Write-Output "Welcome to the Boxstarter Module installer!"
    $args = @(
        "install",  
        "Boxstarter",
        "-y")

    if(![string]::IsNullOrWhiteSpace($Version)) {
        $args += "--version"
        $args += "$VErsion"
    } 
    
    & choco.exe @args 

    if(!(Test-Path "$Env:ProgramData\boxstarter")) {
        Write-Warning "Boxstarter not found at $Env:ProgramData\boxstarter"
        return 
    }

    # 
    Import-Module "$Env:ProgramData\boxstarter\boxstarter.chocolatey\boxstarter.chocolatey.psd1" -Force
}