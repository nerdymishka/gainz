# Copyright © 2017 Chocolatey Software, Inc.
# Copyright © 2011 - 2017 RealDimensions Software, LLC
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
#
# You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# Ideas from the Awesome Posh-Git - https://github.com/dahlbyk/posh-git
# Posh-Git License - https://github.com/dahlbyk/posh-git/blob/1941da2472eb668cde2d6a5fc921d5043a024386/LICENSE.txt
# http://www.jeremyskinner.co.uk/2010/03/07/using-git-with-windows-powershell/

$Global:ChocolateyTabSettings = New-Object PSObject -Property @{
    AllCommands = $false
}

$script:choco = "$env:ChocolateyInstall\choco.exe"

function script:chocoCmdOperations($commands, $command, $filter, $currentArguments) {
    $currentOptions = @('zzzz')
    if ($currentArguments -ne $null -and $currentArguments.Trim() -ne '') { $currentOptions = $currentArguments.Trim() -split ' ' }

    $commands.$command.Replace("  "," ") -split ' ' |
      where { $_ -notmatch "^(?:$($currentOptions -join '|' -replace "=", "\="))(?:\S*)\s?$" } |
      where { $_ -like "$filter*" }
}

$script:someCommands = @('-?','search','list','info','install','outdated','upgrade','uninstall','new','download','pack','push','sync','-h','--help','pin','source','config','feature','apikey')

$allcommands = " --debug --verbose --force --noop --help --accept-license --confirm --limit-output --no-progress --execution-timeout= --cache-location='' --proxy='' --proxy-user= --proxy-password= --proxy-bypass-list='' --proxy-bypass-on-local --fail-on-error-output --use-system-powershell"
$proListOptions = " --audit"
$proInstallUpgradeOptions = " --install-directory='' --max-download-rate= --install-arguments-sensitive= --package-parameters-sensitive= --skip-download-cache --use-download-cache --skip-virus-check --virus-check --virus-positives-minimum="
$proNewOptions = " --file='' --build-package --file64='' --from-programs-and-features --use-original-location --keep-remote --url='' --url64='' --checksum= --checksum64= --checksumtype= --pause-on-error"
$proUninstallOptions = " --from-programs-and-features"
$proPinOptions = " --note=''"

$commandOptions = @{
  list = "--lo --id-only --pre --exact --by-id-only --id-starts-with --detailed --approved-only --not-broken --source='' --user= --password= --local-only --prerelease --include-programs --page= --page-size= --order-by-popularity --download-cache-only" + $proListOptions + $allcommands
  search = "--pre --exact --by-id-only --id-starts-with --detailed --approved-only --not-broken --source='' --user= --password= --local-only --prerelease --include-programs --page= --page-size= --order-by-popularity --download-cache-only" + $allcommands
  info = "--pre --lo --source='' --user= --password= --local-only --prerelease" + $allcommands
  install = "-y -whatif -? --pre --version= --params='' --install-arguments='' --override-arguments --ignore-dependencies --source='' --source='windowsfeatures' --source='webpi' --user= --password= --prerelease --forcex86 --not-silent --package-parameters='' --allow-downgrade --force-dependencies --require-checksums --use-package-exit-codes --ignore-package-exit-codes --skip-automation-scripts --allow-multiple-versions --ignore-checksums --allow-empty-checksums --allow-empty-checksums-secure --download-checksum='' --download-checksum-type='' --download-checksum-x64='' --download-checksum-type-x64='' --stop-on-first-package-failure" + $proInstallUpgradeOptions + $allcommands
  pin = "--name= --version= -?" + $proPinOptions + $allcommands
  outdated = "-? --source='' --user= --password= --ignore-pinned --ignore-unfound" + $allcommands
  upgrade = "-y -whatif -? --pre --version= --except='' --params='' --install-arguments='' --override-arguments --ignore-dependencies --source='' --source='windowsfeatures' --source='webpi' --user= --password= --prerelease --forcex86 --not-silent --package-parameters='' --allow-downgrade --allow-multiple-versions --require-checksums --use-package-exit-codes --ignore-package-exit-codes --skip-automation-scripts --fail-on-unfound --fail-on-not-installed --ignore-checksums --allow-empty-checksums --allow-empty-checksums-secure --download-checksum='' --download-checksum-type='' --download-checksum-x64='' --download-checksum-type-x64='' --exclude-prerelease --stop-on-first-package-failure --use-remembered-options --ignore-remembered-options" + $proInstallUpgradeOptions + $allcommands
  uninstall = "-y -whatif -? --force-dependencies --remove-dependencies --all-versions --source='windowsfeatures' --source='webpi' --version= --uninstall-arguments='' --override-arguments --not-silent --params='' --package-parameters='' --use-package-exit-codes --ignore-package-exit-codes --skip-automation-scripts --use-autouninstaller --skip-autouninstaller --fail-on-autouninstaller --ignore-autouninstaller-failure --stop-on-first-package-failure" + $proUninstallOptions + $allcommands
  new = "--template-name= --output-directory='' --automaticpackage --version= --maintainer='' packageversion= maintainername='' maintainerrepo='' installertype= url='' url64='' silentargs='' --use-built-in-template -?" + $proNewOptions + $allcommands
  pack = "--version= -?" + $allcommands
  push = "--source='' --api-key= --timeout= -?" + $allcommands
  source = "--name= --source='' --user= --password= --priority= --bypass-proxy --allow-self-service -?" + $allcommands
  config = "--name= --value= -?" + $allcommands
  feature = "--name= -?" + $allcommands
  apikey = "--source='' --api-key= -?" + $allcommands
  download = "--internalize --ignore-dependencies --resources-location= --download-location= --outputdirectory= --source='' --version='' --prerelease --user= --password= --cert='' --certpassword= --append-use-original-location --recompile -?" + $allcommands
  sync = "--output-directory= --id= --package-id= -?" + $allcommands
}

try {
  # if license exists
  # add in pro/biz switches
}
catch {
}

function script:chocoCommands($filter) {
    $cmdList = @()
    if (-not $global:ChocolateyTabSettings.AllCommands) {
        $cmdList += $someCommands -like "$filter*"
    } else {
        $cmdList += (& $script:choco -h) |
            where { $_ -match '^  \S.*' } |
            foreach { $_.Split(' ', [StringSplitOptions]::RemoveEmptyEntries) } |
            where { $_ -like "$filter*" }
    }

    $cmdList #| sort
}

function script:chocoLocalPackages($filter) {
    if ($filter -ne $null -and $filter.StartsWith(".")) { return; } #file search
    @(& $script:choco list $filter -lo -r --id-starts-with) | %{ $_.Split('|')[0] }
}

function script:chocoLocalPackagesUpgrade($filter) {
    if ($filter -ne $null -and $filter.StartsWith(".")) { return; } #file search
    @('all|') + @(& $script:choco list $filter -lo -r --id-starts-with) | where { $_ -like "$filter*" } | %{ $_.Split('|')[0] }
}

function script:chocoRemotePackages($filter) {
    if ($filter -ne $null -and $filter.StartsWith(".")) { return; } #file search
    @('packages.config|') + @(& $script:choco search $filter --page=0 --page-size=30 -r --id-starts-with --order-by-popularity) | where { $_ -like "$filter*" } | %{ $_.Split('|')[0] }
}

function Get-AliasPattern($exe) {
  $aliases = @($exe) + @(Get-Alias | where { $_.Definition -eq $exe } | select -Exp Name)

  "($($aliases -join '|'))"
}

function ChocolateyTabExpansion($lastBlock) {
  switch -regex ($lastBlock -replace "^$(Get-AliasPattern choco) ","") {

    # Handles uninstall package names
    "^uninstall\s+(?<package>[^\.][^-\s]*)$" {
      chocoLocalPackages $matches['package']
    }

    # Handles install package names
    "^(install)\s+(?<package>[^\.][^-\s]+)$" {
      chocoRemotePackages $matches['package']
    }

    # Handles upgrade / uninstall package names
    "^upgrade\s+(?<package>[^\.][^-\s]*)$" {
      chocoLocalPackagesUpgrade $matches['package']
    }

    # Handles list/search first tab
    "^(list|search)\s+(?<subcommand>[^-\s]*)$" {
      @('<filter>','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles new first tab
    "^(new)\s+(?<subcommand>[^-\s]*)$" {
      @('<name>','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles pack first tab
    "^(pack)\s+(?<subcommand>[^-\s]*)$" {
      @('<PathtoNuspec>','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles push first tab
    "^(push)\s+(?<subcommand>[^-\s]*)$" {
      @('<PathtoNupkg>','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles source first tab
    "^(source)\s+(?<subcommand>[^-\s]*)$" {
      @('list','add','remove','disable','enable','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles pin first tab
    "^(pin)\s+(?<subcommand>[^-\s]*)$" {
      @('list','add','remove','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles feature first tab
    "^(feature)\s+(?<subcommand>[^-\s]*)$" {
      @('list','disable','enable','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }
    # Handles config first tab
    "^(config)\s+(?<subcommand>[^-\s]*)$" {
      @('list','get','set','unset','-?') | where { $_ -like "$($matches['subcommand'])*" }
    }

    # Handles more options after others
    "^(?<cmd>$($commandOptions.Keys -join '|'))(?<currentArguments>.*)\s+(?<op>\S*)$" {
      chocoCmdOperations $commandOptions $matches['cmd'] $matches['op'] $matches['currentArguments']
    }

    # Handles choco <cmd> <op>
    "^(?<cmd>$($commandOptions.Keys -join '|'))\s+(?<op>\S*)$" {
      chocoCmdOperations $commandOptions $matches['cmd'] $matches['op']
    }

    # Handles choco <cmd>
    "^(?<cmd>\S*)$" {
      chocoCommands $matches['cmd']
    }
  }
}

$PowerTab_RegisterTabExpansion = if (Get-Module -Name powertab) { Get-Command Register-TabExpansion -Module powertab -ErrorAction SilentlyContinue }
if ($PowerTab_RegisterTabExpansion)
{
  & $PowerTab_RegisterTabExpansion "choco" -Type Command {
    param($Context, [ref]$TabExpansionHasOutput, [ref]$QuoteSpaces)  # 1:

    $line = $Context.Line
    $lastBlock = [System.Text.RegularExpressions.Regex]::Split($line, '[|;]')[-1].TrimStart()
    $TabExpansionHasOutput.Value = $true
    ChocolateyTabExpansion $lastBlock
  }

  return
}

if (Test-Path Function:\TabExpansion) {
    Rename-Item Function:\TabExpansion TabExpansionBackup
}

function TabExpansion($line, $lastWord) {
    $lastBlock = [System.Text.RegularExpressions.Regex]::Split($line, '[|;]')[-1].TrimStart()

    switch -regex ($lastBlock) {
        # Execute Chocolatey tab completion for all choco-related commands
        "^$(Get-AliasPattern choco) (.*)" { ChocolateyTabExpansion $lastBlock }

        # Fall back on existing tab expansion
        default { if (Test-Path Function:\TabExpansionBackup) { TabExpansionBackup $line $lastWord } }
    }
}

# SIG # Begin signature block
# MIIcpwYJKoZIhvcNAQcCoIIcmDCCHJQCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCD0whYh2FO5RmyY
# AMDYMituVhALJSfoO9xk7yJuCJHDEKCCF7EwggUwMIIEGKADAgECAhAECRgbX9W7
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
# NwIBFTAvBgkqhkiG9w0BCQQxIgQgFvC9KYL/NnmS4QfGMjOZDL9F/ZkCwFxw5hgG
# 0SZYNi4wDQYJKoZIhvcNAQEBBQAEggEAp2fswD3IFvqlnFE4HEDzitquhJGr0vOL
# RhLz9n9XvLZIhtA+5VCWUuZgDBfNmtfYzz9lZUYF3Ik2PiaJ6nV55dGBx2eyFhFH
# QfDVhbrntSTu2whiASmUnMy2Oh4YDqvOaXYgA4aoWmOzZSOKh1aGHYz53CVLnRY1
# ByUzRnesJFdZomXZlGk+B1lpxONhJo6SqZ6AwM5TBSfeYleuutYio09+ECJgwboD
# SG0gI4dMIjUVJ7YV83bpD46X39+H6eGyMwKS8Zv2tVO+aZs04ID+U5UrJPQasKW+
# Csvo9lxckvombBG8VIDHfyqVx+0h0uoblwoJ+wy/MJvbIxETY+mpUKGCAg8wggIL
# BgkqhkiG9w0BCQYxggH8MIIB+AIBATB2MGIxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xITAfBgNV
# BAMTGERpZ2lDZXJ0IEFzc3VyZWQgSUQgQ0EtMQIQAwGaAjr/WLFr1tXq5hfwZjAJ
# BgUrDgMCGgUAoF0wGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkqhkiG9w0B
# CQUxDxcNMTkwMzE1MTc1NzAzWjAjBgkqhkiG9w0BCQQxFgQU7fm6WxvmBqqC4HH0
# iDhPkZkykZ4wDQYJKoZIhvcNAQEBBQAEggEAjT3dK8eBc6FfySij+/IDonXPIknv
# JIHn0EdOgR5m5ra+0vfAtwrduT1OXFe6gmDRsX+by8tuDGzxYWZhT+AvwPkS7rL3
# DTvYMTo8+WBa6KTQ+26AMkTGIQMSaSbBdFDvYBFs7VRtH+l9a6uVNRDZnEBGVLBo
# WO/E+d+q3Dl4h1swDW2v0Qh1p024qK3maXQ8HNtdqS4gFXS6gpvBC+OeBw/MuXfU
# x+LDk2HyBk7cwHk5Ds2taADvu2u4OqnMdFBlVOMBvQQSWvwH5O5uWQ3FAjbh4oUf
# VT0Lo/xRg9AW+NDCe7ciKiPH3uo/KMMsYKAZlmEEwoo08HhKAMdAAyw80A==
# SIG # End signature block
