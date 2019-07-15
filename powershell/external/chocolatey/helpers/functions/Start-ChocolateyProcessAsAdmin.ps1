# Copyright © 2017 Chocolatey Software, Inc.
# Copyright © 2015 - 2017 RealDimensions Software, LLC
# Copyright © 2011 - 2015 RealDimensions Software, LLC & original authors/contributors from https://github.com/chocolatey/chocolatey
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

function Start-ChocolateyProcessAsAdmin {
<#
.SYNOPSIS
**NOTE:** Administrative Access Required.

Runs a process with administrative privileges. If `-ExeToRun` is not
specified, it is run with PowerShell.

.NOTES
This command will assert UAC/Admin privileges on the machine.

Starting in 0.9.10, will automatically call Set-PowerShellExitCode to
set the package exit code in the following ways:

- 4 if the binary turns out to be a text file.
- The same exit code returned from the process that is run. If a 3010 is returned, it will set 3010 for the package.

Aliases `Start-ChocolateyProcess` and `Invoke-ChocolateyProcess`
available in 0.10.2+.

.INPUTS
None

.OUTPUTS
None

.PARAMETER Statements
Arguments to pass to `ExeToRun` or the PowerShell script block to be
run.

.PARAMETER ExeToRun
The executable/application/installer to run. Defaults to `'powershell'`.

.PARAMETER Elevated
Indicate whether the process should run elevated.

Available in 0.10.2+.

.PARAMETER Minimized
Switch indicating if a Windows pops up (if not called with a silent
argument) that it should be minimized.

.PARAMETER NoSleep
Used only when calling PowerShell - indicates the window that is opened
should return instantly when it is complete.

.PARAMETER ValidExitCodes
Array of exit codes indicating success. Defaults to `@(0)`.

.PARAMETER WorkingDirectory
The working directory for the running process. Defaults to
`Get-Location`.

Available in 0.10.1+.

.PARAMETER SensitiveStatements
Arguments to pass to  `ExeToRun` that are not logged.

Note that only licensed versions of Chocolatey provide a way to pass
those values completely through without having them in the install
script or on the system in some way.

Available in 0.10.1+.

.PARAMETER IgnoredArguments
Allows splatting with arguments that do not apply. Do not use directly.

.EXAMPLE
Start-ChocolateyProcessAsAdmin -Statements "$msiArgs" -ExeToRun 'msiexec'

.EXAMPLE
Start-ChocolateyProcessAsAdmin -Statements "$silentArgs" -ExeToRun $file

.EXAMPLE
Start-ChocolateyProcessAsAdmin -Statements "$silentArgs" -ExeToRun $file -ValidExitCodes @(0,21)

.EXAMPLE
>
# Run PowerShell statements
$psFile = Join-Path "$(Split-Path -parent $MyInvocation.MyCommand.Definition)" 'someInstall.ps1'
Start-ChocolateyProcessAsAdmin "& `'$psFile`'"

.EXAMPLE
# This also works for cmd and is required if you have any spaces in the paths within your command
$appPath = "$env:ProgramFiles\myapp"
$cmdBatch = "/c `"$appPath\bin\installmyappservice.bat`""
Start-ChocolateyProcessAsAdmin $cmdBatch cmd
# or more explicitly
Start-ChocolateyProcessAsAdmin -Statements $cmdBatch -ExeToRun "cmd.exe"

.LINK
Install-ChocolateyPackage

.LINK
Install-ChocolateyInstallPackage
#>
param(
  [parameter(Mandatory=$false, Position=0)][string[]] $statements,
  [parameter(Mandatory=$false, Position=1)][string] $exeToRun = 'powershell',
  [parameter(Mandatory=$false)][switch] $elevated = $true,
  [parameter(Mandatory=$false)][switch] $minimized,
  [parameter(Mandatory=$false)][switch] $noSleep,
  [parameter(Mandatory=$false)] $validExitCodes = @(0),
  [parameter(Mandatory=$false)][string] $workingDirectory = $(Get-Location),
  [parameter(Mandatory=$false)][string] $sensitiveStatements = '',
  [parameter(ValueFromRemainingArguments = $true)][Object[]] $ignoredArguments
)
  [string]$statements = $statements -join ' '

  Write-FunctionCallLogMessage -Invocation $MyInvocation -Parameters $PSBoundParameters

  $alreadyElevated = $false
  if (Test-ProcessAdminRights) {
    $alreadyElevated = $true
  }

  $dbMessagePrepend = "Elevating permissions and running"
  if (!$elevated) {
    $dbMessagePrepend = "Running"
  }

  try {
    if ($exeToRun -ne $null) { $exeToRun = $exeToRun -replace "`0", "" }
    if ($statements -ne $null) { $statements = $statements -replace "`0", "" }
  } catch {
    Write-Debug "Removing null characters resulted in an error - $($_.Exception.Message)"
  }

  if ($exeToRun -ne $null) {
    $exeToRun = $exeToRun.Trim().Trim("'").Trim('"')
  }

  $wrappedStatements = $statements
  if ($wrappedStatements -eq $null) { $wrappedStatements = ''}

  if ($exeToRun -eq 'powershell') {
    $exeToRun = "$($env:SystemRoot)\System32\WindowsPowerShell\v1.0\powershell.exe"
    $importChocolateyHelpers = "& import-module -name '$helpersPath\chocolateyInstaller.psm1' -Verbose:`$false | Out-Null;"
    $block = @"
      `$noSleep = `$$noSleep
      #`$env:ChocolateyEnvironmentDebug='false'
      #`$env:ChocolateyEnvironmentVerbose='false'
      $importChocolateyHelpers
      try{
        `$progressPreference="SilentlyContinue"
        $statements
        if(!`$noSleep){start-sleep 6}
      }
      catch{
        if(!`$noSleep){start-sleep 8}
        throw
      }
"@
    $encoded = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($block))
    $wrappedStatements = "-NoLogo -NonInteractive -NoProfile -ExecutionPolicy Bypass -InputFormat Text -OutputFormat Text -EncodedCommand $encoded"
    $dbgMessage = @"
$dbMessagePrepend powershell block:
$block
This may take a while, depending on the statements.
"@

  }
  else
  {
    $dbgMessage = @"
$dbMessagePrepend [`"$exeToRun`" $wrappedStatements]. This may take a while, depending on the statements.
"@
  }

  Write-Debug $dbgMessage

  $exeIsTextFile = [System.IO.Path]::GetFullPath($exeToRun) + ".istext"
  if ([System.IO.File]::Exists($exeIsTextFile)) {
    Set-PowerShellExitCode 4
    throw "The file was a text file but is attempting to be run as an executable - '$exeToRun'"
  }

  if ($exeToRun -eq 'msiexec' -or $exeToRun -eq 'msiexec.exe') {
    $exeToRun = "$($env:SystemRoot)\System32\msiexec.exe"
  }

  if (!([System.IO.File]::Exists($exeToRun)) -and $exeToRun -notmatch 'msiexec') {
    Write-Warning "May not be able to find '$exeToRun'. Please use full path for executables."
    # until we have search paths enabled, let's just pass a warning
    #Set-PowerShellExitCode 2
    #throw "Could not find '$exeToRun'"
  }

  # Redirecting output slows things down a bit.
  $writeOutput = {
    if ($EventArgs.Data -ne $null) {
      Write-Verbose "$($EventArgs.Data)"
    }
  }

  $writeError = {
    if ($EventArgs.Data -ne $null) {
      Write-Error "$($EventArgs.Data)"
    }
  }

  $process = New-Object System.Diagnostics.Process
  $process.EnableRaisingEvents = $true
  Register-ObjectEvent -InputObject $process -SourceIdentifier "LogOutput_ChocolateyProc" -EventName OutputDataReceived -Action $writeOutput | Out-Null
  Register-ObjectEvent -InputObject $process -SourceIdentifier "LogErrors_ChocolateyProc" -EventName ErrorDataReceived -Action  $writeError | Out-Null

  #$process.StartInfo = New-Object System.Diagnostics.ProcessStartInfo($exeToRun, $wrappedStatements)
  # in case empty args makes a difference, try to be compatible with the older
  # version
  $psi = New-Object System.Diagnostics.ProcessStartInfo

  $psi.FileName = $exeToRun
  if ($wrappedStatements -ne '') {
    $psi.Arguments = "$wrappedStatements"
  }
  if ($sensitiveStatements -ne $null -and $sensitiveStatements -ne '') {
    Write-Host "Sensitive arguments have been passed. Adding to arguments."
    $psi.Arguments += " $sensitiveStatements"
  }
  $process.StartInfo =  $psi

  # process start info
  $process.StartInfo.RedirectStandardOutput = $true
  $process.StartInfo.RedirectStandardError = $true
  $process.StartInfo.UseShellExecute = $false
  $process.StartInfo.WorkingDirectory = $workingDirectory

  if ($elevated -and -not $alreadyElevated -and [Environment]::OSVersion.Version -ge (New-Object 'Version' 6,0)){
    # this doesn't actually currently work - because we are not running under shell execute
    Write-Debug "Setting RunAs for elevation"
    $process.StartInfo.Verb = "RunAs"
  }
  if ($minimized) {
    $process.StartInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Minimized
  }

  $process.Start() | Out-Null
  if ($process.StartInfo.RedirectStandardOutput) { $process.BeginOutputReadLine() }
  if ($process.StartInfo.RedirectStandardError) { $process.BeginErrorReadLine() }
  $process.WaitForExit()

  # For some reason this forces the jobs to finish and waits for
  # them to do so. Without this it never finishes.
  Unregister-Event -SourceIdentifier "LogOutput_ChocolateyProc"
  Unregister-Event -SourceIdentifier "LogErrors_ChocolateyProc"

  # sometimes the process hasn't fully exited yet.
  for ($loopCount=1; $loopCount -le 15; $loopCount++) {
    if ($process.HasExited) { break; }
    Write-Debug "Waiting for process to exit - $loopCount/15 seconds";
    Start-Sleep 1;
  }

  $exitCode = $process.ExitCode
  $process.Dispose()

  Write-Debug "Command [`"$exeToRun`" $wrappedStatements] exited with `'$exitCode`'."

  $exitErrorMessage = ''
  $errorMessageAddendum = " This is most likely an issue with the '$env:chocolateyPackageName' package and not with Chocolatey itself. Please follow up with the package maintainer(s) directly."

  switch ($exitCode) {
    0 { break }
    1 { break }
    3010 { break }
    # NSIS - http://nsis.sourceforge.net/Docs/AppendixD.html
    # InnoSetup - http://www.jrsoftware.org/ishelp/index.php?topic=setupexitcodes
    2 { $exitErrorMessage = 'Setup was cancelled.'; break }
    3 { $exitErrorMessage = 'A fatal error occurred when preparing or moving to next install phase. Check to be sure you have enough memory to perform an installation and try again.'; break }
    4 { $exitErrorMessage = 'A fatal error occurred during installation process.' + $errorMessageAddendum; break }
    5 { $exitErrorMessage = 'User (you) cancelled the installation.'; break }
    6 { $exitErrorMessage = 'Setup process was forcefully terminated by the debugger.'; break }
    7 { $exitErrorMessage = 'While preparing to install, it was determined setup cannot proceed with the installation. Please be sure the software can be installed on your system.'; break }
    8 { $exitErrorMessage = 'While preparing to install, it was determined setup cannot proceed with the installation until you restart the system. Please reboot and try again.'; break }
    # MSI - https://msdn.microsoft.com/en-us/library/windows/desktop/aa376931.aspx
    1602 { $exitErrorMessage = 'User (you) cancelled the installation.'; break }
    1603 { $exitErrorMessage = "Generic MSI Error. This is a local environment error, not an issue with a package or the MSI itself - it could mean a pending reboot is necessary prior to install or something else (like the same version is already installed). Please see MSI log if available. If not, try again adding `'--install-arguments=`"`'/l*v c:\$($env:chocolateyPackageName)_msi_install.log`'`"`'. Then search the MSI Log for `"Return Value 3`" and look above that for the error."; break }
    1618 { $exitErrorMessage = 'Another installation currently in progress. Try again later.'; break }
    1619 { $exitErrorMessage = 'MSI could not be found - it is possibly corrupt or not an MSI at all. If it was downloaded and the MSI is less than 30K, try opening it in an editor like Notepad++ as it is likely HTML.' + $errorMessageAddendum; break }
    1620 { $exitErrorMessage = 'MSI could not be opened - it is possibly corrupt or not an MSI at all. If it was downloaded and the MSI is less than 30K, try opening it in an editor like Notepad++ as it is likely HTML.' + $errorMessageAddendum; break }
    1622 { $exitErrorMessage = 'Something is wrong with the install log location specified. Please fix this in the package silent arguments (or in install arguments you specified). The directory specified as part of the log file path must exist for an MSI to be able to log to that directory.' + $errorMessageAddendum; break }
    1623 { $exitErrorMessage = 'This MSI has a language that is not supported by your system. Contact package maintainer(s) if there is an install available in your language and you would like it added to the packaging.'; break }
    1625 { $exitErrorMessage = 'Installation of this MSI is forbidden by system policy. Please contact your system administrators.'; break }
    1632 { $exitErrorMessage = 'Installation of this MSI is not supported on this platform. Contact package maintainer(s) if you feel this is in error or if you need an architecture that is not available with the current packaging.'; break }
    1633 { $exitErrorMessage = 'Installation of this MSI is not supported on this platform. Contact package maintainer(s) if you feel this is in error or if you need an architecture that is not available with the current packaging.'; break }
    1638 { $exitErrorMessage = 'This MSI requires uninstall prior to installing a different version. Please ask the package maintainer(s) to add a check in the chocolateyInstall.ps1 script and uninstall if the software is installed.' + $errorMessageAddendum; break }
    1639 { $exitErrorMessage = 'The command line arguments passed to the MSI are incorrect. If you passed in additional arguments, please adjust. Otherwise followup with the package maintainer(s) to get this fixed.' + $errorMessageAddendum; break }
    1640 { $exitErrorMessage = 'Cannot install MSI when running from remote desktop (terminal services). This should automatically be handled in licensed editions. For open source editions, you may need to run change.exe prior to running Chocolatey or not use terminal services.'; break }
    1645 { $exitErrorMessage = 'Cannot install MSI when running from remote desktop (terminal services). This should automatically be handled in licensed editions. For open source editions, you may need to run change.exe prior to running Chocolatey or not use terminal services.'; break }
  }

  if ($exitErrorMessage) {
    $errorMessageSpecific = "Exit code indicates the following: $exitErrorMessage."
    Write-Warning $exitErrorMessage
  } else {
    $errorMessageSpecific = 'See log for possible error messages.'
  }

  if ($validExitCodes -notcontains $exitCode) {
    Set-PowerShellExitCode $exitCode
    throw "Running [`"$exeToRun`" $wrappedStatements] was not successful. Exit code was '$exitCode'. $($errorMessageSpecific)"
  } else {
    $chocoSuccessCodes = @(0, 1605, 1614, 1641, 3010)
    if ($chocoSuccessCodes -notcontains $exitCode) {
      Write-Warning "Exit code '$exitCode' was considered valid by script, but not as a Chocolatey success code. Returning '0'."
      $exitCode = 0
    }
  }

  Write-Debug "Finishing '$($MyInvocation.InvocationName)'"

  return $exitCode
}

Set-Alias Start-ChocolateyProcess Start-ChocolateyProcessAsAdmin
Set-Alias Invoke-ChocolateyProcess Start-ChocolateyProcessAsAdmin

# SIG # Begin signature block
# MIIcpwYJKoZIhvcNAQcCoIIcmDCCHJQCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCBM0B9N/gorGD8+
# +9Lu/ixd9N7cDuI0Q10kf6LMTBsNvqCCF7EwggUwMIIEGKADAgECAhAECRgbX9W7
# ZnVTQ7VvlVAIMA0GCSqGSIb3DQEBCwUAMGUxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xJDAiBgNV
# BAMTG0RpZ2lDZXJ0IEFzc3VyZWQgSUQgUm9vdCBDQTAeFw0xMzEwMjIxMjAwMDBa
# Fw0yODEwMjIxMjAwMDBaMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2Vy
# dCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lD
# ZXJ0IFNIQTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0EwggEiMA0GCSqGSIb3
# DQEBAQUAA4IBDwAwggEKAoIBAQD407Mcfw4Rr2d3B9MLMUkZz9D7RZmxOttE9X/l
# qJ3bMtdx6nadBS63j/qSQ8Cl+YnUNxnXtqrwnIal2CWsDnkoOn7p0WfTxvspJ8fT
# eyOU5JEjlpB3gvmhhCNmElQzUHSxKCa7JGnCwlLyFGeKiUXULaGj6YgsIJWuHEqH
# CN8M9eJNYBi+qsSyrnAxZjNxPqxwoqvOf+l8y5Kh5TsxHM/q8grkV7tKtel05iv+
# bMt+dDk2DZDv5LVOpKnqagqrhPOsZ061xPeM0SAlI+sIZD5SlsHyDxL0xY4PwaLo
# LFH3c7y9hbFig3NBggfkOItqcyDQD2RzPJ6fpjOp/RnfJZPRAgMBAAGjggHNMIIB
# yTASBgNVHRMBAf8ECDAGAQH/AgEAMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUEDDAK
# BggrBgEFBQcDAzB5BggrBgEFBQcBAQRtMGswJAYIKwYBBQUHMAGGGGh0dHA6Ly9v
# Y3NwLmRpZ2ljZXJ0LmNvbTBDBggrBgEFBQcwAoY3aHR0cDovL2NhY2VydHMuZGln
# aWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNydDCBgQYDVR0fBHow
# eDA6oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJl
# ZElEUm9vdENBLmNybDA6oDigNoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0Rp
# Z2lDZXJ0QXNzdXJlZElEUm9vdENBLmNybDBPBgNVHSAESDBGMDgGCmCGSAGG/WwA
# AgQwKjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzAK
# BghghkgBhv1sAzAdBgNVHQ4EFgQUWsS5eyoKo6XqcQPAYPkt9mV1DlgwHwYDVR0j
# BBgwFoAUReuir/SSy4IxLVGLp6chnfNtyA8wDQYJKoZIhvcNAQELBQADggEBAD7s
# DVoks/Mi0RXILHwlKXaoHV0cLToaxO8wYdd+C2D9wz0PxK+L/e8q3yBVN7Dh9tGS
# dQ9RtG6ljlriXiSBThCk7j9xjmMOE0ut119EefM2FAaK95xGTlz/kLEbBw6RFfu6
# r7VRwo0kriTGxycqoSkoGjpxKAI8LpGjwCUR4pwUR6F6aGivm6dcIFzZcbEMj7uo
# +MUSaJ/PQMtARKUT8OZkDCUIQjKyNookAv4vcn4c10lFluhZHen6dGRrsutmQ9qz
# sIzV6Q3d9gEgzpkxYz0IGhizgZtPxpMQBvwHgfqL2vmCSfdibqFT+hKUGIUukpHq
# aGxEMrJmoecYpJpkUe8wggU6MIIEIqADAgECAhAH+0XZ9wtVKQNgl7T04UNwMA0G
# CSqGSIb3DQEBCwUAMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJ
# bmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lDZXJ0
# IFNIQTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0EwHhcNMTgwMzMwMDAwMDAw
# WhcNMjEwNDE0MTIwMDAwWjB3MQswCQYDVQQGEwJVUzEPMA0GA1UECBMGS2Fuc2Fz
# MQ8wDQYDVQQHEwZUb3Bla2ExIjAgBgNVBAoTGUNob2NvbGF0ZXkgU29mdHdhcmUs
# IEluYy4xIjAgBgNVBAMTGUNob2NvbGF0ZXkgU29mdHdhcmUsIEluYy4wggEiMA0G
# CSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC4irdLWVJryfKSgPPCyMN+nBmxtZIm
# mTBhJMaYVJ6gtfvHcFakH7IC8TcjcEIrkK7wB/2vEJkEqiOTgbVQPZLnfX8ZAxhd
# UiJmwQHEiSwLzoo2B35ROQ9qdOsn1bYIEzDpaqm/XwYH925LLpxhr9oCkBNf5dZs
# e5bc/s1J5sQ9HRYwpb3MimmNHGpNP/YhjXX/kNFCZIv3mUadFHi+talYIN5dp6ai
# /k+qgZeL5klPdmjyIgf3JiDywCf7j5nSbm3sWarYjM5vLe/oD+eK70fez30a17Cy
# 97Jtqmdz6WUV1BcbMWeb9b8x369UJq5vt7vGwVFDOeGjwffuVHLRvWLnAgMBAAGj
# ggHFMIIBwTAfBgNVHSMEGDAWgBRaxLl7KgqjpepxA8Bg+S32ZXUOWDAdBgNVHQ4E
# FgQUqRlYCMLOvsDUS4mx9UA1avD3fvgwDgYDVR0PAQH/BAQDAgeAMBMGA1UdJQQM
# MAoGCCsGAQUFBwMDMHcGA1UdHwRwMG4wNaAzoDGGL2h0dHA6Ly9jcmwzLmRpZ2lj
# ZXJ0LmNvbS9zaGEyLWFzc3VyZWQtY3MtZzEuY3JsMDWgM6Axhi9odHRwOi8vY3Js
# NC5kaWdpY2VydC5jb20vc2hhMi1hc3N1cmVkLWNzLWcxLmNybDBMBgNVHSAERTBD
# MDcGCWCGSAGG/WwDATAqMCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2Vy
# dC5jb20vQ1BTMAgGBmeBDAEEATCBhAYIKwYBBQUHAQEEeDB2MCQGCCsGAQUFBzAB
# hhhodHRwOi8vb2NzcC5kaWdpY2VydC5jb20wTgYIKwYBBQUHMAKGQmh0dHA6Ly9j
# YWNlcnRzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydFNIQTJBc3N1cmVkSURDb2RlU2ln
# bmluZ0NBLmNydDAMBgNVHRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IBAQA+ddcs
# z/NB/+V+AIlUNOVTlGDNCtn1AfvwoRZg9XMmx0/S0EKayfVFTk/x96WMQgxL+/5x
# B8Uhw6anlhbPC6bjBcIxRj/IUgR7yJ/NAykyM1x+pWvkPZV3slwe0GDPwhaqGUTU
# aG8njO4EvA682a1o7wqQFR1MIltjtuPB2gp311LLxP1k5dpUMgaA0lAfnbRr+5dc
# QOFWslkho1eBf0xlzSrhRGPy0e/IYWpl+/sEwXhD88QUkN7dSXY0fMlyGQfn6H4f
# ozBQvCk37eoE0uAtkUrWAlJxO/4Esi83ko4hokwQJHaN64/7NdNaKlG3shC9+2QM
# kY3j3BU+Ym2GZgtBMIIGajCCBVKgAwIBAgIQAwGaAjr/WLFr1tXq5hfwZjANBgkq
# hkiG9w0BAQUFADBiMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5j
# MRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMSEwHwYDVQQDExhEaWdpQ2VydCBB
# c3N1cmVkIElEIENBLTEwHhcNMTQxMDIyMDAwMDAwWhcNMjQxMDIyMDAwMDAwWjBH
# MQswCQYDVQQGEwJVUzERMA8GA1UEChMIRGlnaUNlcnQxJTAjBgNVBAMTHERpZ2lD
# ZXJ0IFRpbWVzdGFtcCBSZXNwb25kZXIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAw
# ggEKAoIBAQCjZF38fLPggjXg4PbGKuZJdTvMbuBTqZ8fZFnmfGt/a4ydVfiS457V
# WmNbAklQ2YPOb2bu3cuF6V+l+dSHdIhEOxnJ5fWRn8YUOawk6qhLLJGJzF4o9GS2
# ULf1ErNzlgpno75hn67z/RJ4dQ6mWxT9RSOOhkRVfRiGBYxVh3lIRvfKDo2n3k5f
# 4qi2LVkCYYhhchhoubh87ubnNC8xd4EwH7s2AY3vJ+P3mvBMMWSN4+v6GYeofs/s
# jAw2W3rBerh4x8kGLkYQyI3oBGDbvHN0+k7Y/qpA8bLOcEaD6dpAoVk62RUJV5lW
# MJPzyWHM0AjMa+xiQpGsAsDvpPCJEY93AgMBAAGjggM1MIIDMTAOBgNVHQ8BAf8E
# BAMCB4AwDAYDVR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcDCDCCAb8G
# A1UdIASCAbYwggGyMIIBoQYJYIZIAYb9bAcBMIIBkjAoBggrBgEFBQcCARYcaHR0
# cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzCCAWQGCCsGAQUFBwICMIIBVh6CAVIA
# QQBuAHkAIAB1AHMAZQAgAG8AZgAgAHQAaABpAHMAIABDAGUAcgB0AGkAZgBpAGMA
# YQB0AGUAIABjAG8AbgBzAHQAaQB0AHUAdABlAHMAIABhAGMAYwBlAHAAdABhAG4A
# YwBlACAAbwBmACAAdABoAGUAIABEAGkAZwBpAEMAZQByAHQAIABDAFAALwBDAFAA
# UwAgAGEAbgBkACAAdABoAGUAIABSAGUAbAB5AGkAbgBnACAAUABhAHIAdAB5ACAA
# QQBnAHIAZQBlAG0AZQBuAHQAIAB3AGgAaQBjAGgAIABsAGkAbQBpAHQAIABsAGkA
# YQBiAGkAbABpAHQAeQAgAGEAbgBkACAAYQByAGUAIABpAG4AYwBvAHIAcABvAHIA
# YQB0AGUAZAAgAGgAZQByAGUAaQBuACAAYgB5ACAAcgBlAGYAZQByAGUAbgBjAGUA
# LjALBglghkgBhv1sAxUwHwYDVR0jBBgwFoAUFQASKxOYspkH7R7for5XDStnAs0w
# HQYDVR0OBBYEFGFaTSS2STKdSip5GoNL9B6Jwcp9MH0GA1UdHwR2MHQwOKA2oDSG
# Mmh0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJRENBLTEu
# Y3JsMDigNqA0hjJodHRwOi8vY3JsNC5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1
# cmVkSURDQS0xLmNybDB3BggrBgEFBQcBAQRrMGkwJAYIKwYBBQUHMAGGGGh0dHA6
# Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBBBggrBgEFBQcwAoY1aHR0cDovL2NhY2VydHMu
# ZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEQ0EtMS5jcnQwDQYJKoZIhvcN
# AQEFBQADggEBAJ0lfhszTbImgVybhs4jIA+Ah+WI//+x1GosMe06FxlxF82pG7xa
# FjkAneNshORaQPveBgGMN/qbsZ0kfv4gpFetW7easGAm6mlXIV00Lx9xsIOUGQVr
# NZAQoHuXx/Y/5+IRQaa9YtnwJz04HShvOlIJ8OxwYtNiS7Dgc6aSwNOOMdgv420X
# Ewbu5AO2FKvzj0OncZ0h3RTKFV2SQdr5D4HRmXQNJsQOfxu19aDxxncGKBXp2JPl
# VRbwuwqrHNtcSCdmyKOLChzlldquxC5ZoGHd2vNtomHpigtt7BIYvfdVVEADkitr
# wlHCCkivsNRu4PQUCjob4489yq9qjXvc2EQwggbNMIIFtaADAgECAhAG/fkDlgOt
# 6gAK6z8nu7obMA0GCSqGSIb3DQEBBQUAMGUxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xJDAiBgNV
# BAMTG0RpZ2lDZXJ0IEFzc3VyZWQgSUQgUm9vdCBDQTAeFw0wNjExMTAwMDAwMDBa
# Fw0yMTExMTAwMDAwMDBaMGIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2Vy
# dCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xITAfBgNVBAMTGERpZ2lD
# ZXJ0IEFzc3VyZWQgSUQgQ0EtMTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoC
# ggEBAOiCLZn5ysJClaWAc0Bw0p5WVFypxNJBBo/JM/xNRZFcgZ/tLJz4FlnfnrUk
# FcKYubR3SdyJxArar8tea+2tsHEx6886QAxGTZPsi3o2CAOrDDT+GEmC/sfHMUiA
# fB6iD5IOUMnGh+s2P9gww/+m9/uizW9zI/6sVgWQ8DIhFonGcIj5BZd9o8dD3QLo
# Oz3tsUGj7T++25VIxO4es/K8DCuZ0MZdEkKB4YNugnM/JksUkK5ZZgrEjb7Szgau
# rYRvSISbT0C58Uzyr5j79s5AXVz2qPEvr+yJIvJrGGWxwXOt1/HYzx4KdFxCuGh+
# t9V3CidWfA9ipD8yFGCV/QcEogkCAwEAAaOCA3owggN2MA4GA1UdDwEB/wQEAwIB
# hjA7BgNVHSUENDAyBggrBgEFBQcDAQYIKwYBBQUHAwIGCCsGAQUFBwMDBggrBgEF
# BQcDBAYIKwYBBQUHAwgwggHSBgNVHSAEggHJMIIBxTCCAbQGCmCGSAGG/WwAAQQw
# ggGkMDoGCCsGAQUFBwIBFi5odHRwOi8vd3d3LmRpZ2ljZXJ0LmNvbS9zc2wtY3Bz
# LXJlcG9zaXRvcnkuaHRtMIIBZAYIKwYBBQUHAgIwggFWHoIBUgBBAG4AeQAgAHUA
# cwBlACAAbwBmACAAdABoAGkAcwAgAEMAZQByAHQAaQBmAGkAYwBhAHQAZQAgAGMA
# bwBuAHMAdABpAHQAdQB0AGUAcwAgAGEAYwBjAGUAcAB0AGEAbgBjAGUAIABvAGYA
# IAB0AGgAZQAgAEQAaQBnAGkAQwBlAHIAdAAgAEMAUAAvAEMAUABTACAAYQBuAGQA
# IAB0AGgAZQAgAFIAZQBsAHkAaQBuAGcAIABQAGEAcgB0AHkAIABBAGcAcgBlAGUA
# bQBlAG4AdAAgAHcAaABpAGMAaAAgAGwAaQBtAGkAdAAgAGwAaQBhAGIAaQBsAGkA
# dAB5ACAAYQBuAGQAIABhAHIAZQAgAGkAbgBjAG8AcgBwAG8AcgBhAHQAZQBkACAA
# aABlAHIAZQBpAG4AIABiAHkAIAByAGUAZgBlAHIAZQBuAGMAZQAuMAsGCWCGSAGG
# /WwDFTASBgNVHRMBAf8ECDAGAQH/AgEAMHkGCCsGAQUFBwEBBG0wazAkBggrBgEF
# BQcwAYYYaHR0cDovL29jc3AuZGlnaWNlcnQuY29tMEMGCCsGAQUFBzAChjdodHRw
# Oi8vY2FjZXJ0cy5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0Eu
# Y3J0MIGBBgNVHR8EejB4MDqgOKA2hjRodHRwOi8vY3JsMy5kaWdpY2VydC5jb20v
# RGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3JsMDqgOKA2hjRodHRwOi8vY3JsNC5k
# aWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3JsMB0GA1UdDgQW
# BBQVABIrE5iymQftHt+ivlcNK2cCzTAfBgNVHSMEGDAWgBRF66Kv9JLLgjEtUYun
# pyGd823IDzANBgkqhkiG9w0BAQUFAAOCAQEARlA+ybcoJKc4HbZbKa9Sz1LpMUer
# Vlx71Q0LQbPv7HUfdDjyslxhopyVw1Dkgrkj0bo6hnKtOHisdV0XFzRyR4WUVtHr
# uzaEd8wkpfMEGVWp5+Pnq2LN+4stkMLA0rWUvV5PsQXSDj0aqRRbpoYxYqioM+Sb
# OafE9c4deHaUJXPkKqvPnHZL7V/CSxbkS3BMAIke/MV5vEwSV/5f4R68Al2o/vsH
# OE8Nxl2RuQ9nRc3Wg+3nkg2NsWmMT/tZ4CMP0qquAHzunEIOz5HXJ7cW7g/DvXwK
# oO4sCFWFIrjrGBpN/CohrUkxg0eVd3HcsRtLSxwQnHcUwZ1PL1qVCCkQJjGCBEww
# ggRIAgEBMIGGMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMx
# GTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lDZXJ0IFNI
# QTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0ECEAf7Rdn3C1UpA2CXtPThQ3Aw
# DQYJYIZIAWUDBAIBBQCggYQwGAYKKwYBBAGCNwIBDDEKMAigAoAAoQKAADAZBgkq
# hkiG9w0BCQMxDAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4wDAYKKwYBBAGC
# NwIBFTAvBgkqhkiG9w0BCQQxIgQgLXLfDm5ZO0F7n2Nexf5QBKk9rmoDUthxG+r4
# F9i69HgwDQYJKoZIhvcNAQEBBQAEggEAb2mru9l/KwdisLhftWBpxN2YplsjnNO8
# zOerUDH77YZLnOp6pfxlw9UDk6p2H7ZSf3PJz+1J4ENwf5p+TfyvqyZyYXPQ32to
# qWgjxlTneLaqxbY8CnJe+4NWxDAEhbhSMObYX29KYVIq+iJ1q9yYpR2pNuE+/PAq
# I3FXS45kX4WtsOfVnkJnWM/oIKKfEuywPi+a6l4dy/jMsEP3rzCdHjJIziLcIJ34
# lSv2YwwyUk8CYmQfWMAFLZfhH1Y8jmL6IR59aaSJ3oJxesogSfNvDK+LLc6tch7I
# enySIVHhp4EaIijUSmbziY7zSHwkwnEwMWOKWcUxUQltJ+Rw/85ynKGCAg8wggIL
# BgkqhkiG9w0BCQYxggH8MIIB+AIBATB2MGIxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xITAfBgNV
# BAMTGERpZ2lDZXJ0IEFzc3VyZWQgSUQgQ0EtMQIQAwGaAjr/WLFr1tXq5hfwZjAJ
# BgUrDgMCGgUAoF0wGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkqhkiG9w0B
# CQUxDxcNMTkwMzE1MTc1NzAwWjAjBgkqhkiG9w0BCQQxFgQUumRD/TbOT5Vq6kjT
# F5SC4Ge0EbQwDQYJKoZIhvcNAQEBBQAEggEADeiy6pwgzhNhQoFY5dlA/QDn7A/Z
# +0d5nogna5UjTzViKkhfAEXFS7/3Y0MNKSu7K9ypaZ5nSaBImxkGX40IAN0yzsU1
# RpA4GbDCp7JpLvFPmKBnuAKTOgvAr1b+wbojbWa24lAF/8gtvWw8Y4kjKQ3gvtPz
# bpZWF+2pp5JBG+dpRHLizYL0ypB6Q/y0Vk9D3UvkdIpTfNG4OFZXxLVeh5/2V192
# LiKSonbjK4o8DXy85RvSk46BG2n1fy5M5vDQZZE6NAN61k306Jz15G76zQZ7vQx8
# bKdBLHWQb5jMK5+3EetGt+PpBoSAXLmYWaKKOojWHMUWnAYUf5kILPdPSA==
# SIG # End signature block
