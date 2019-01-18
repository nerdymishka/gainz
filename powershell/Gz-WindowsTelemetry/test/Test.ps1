Import-Module "$PSScriptRoot/../Gz-WindowsTelemetry.psm1" -Force

Describe "Gz-WindowsTelemetry" {

    IT "Should invoke TeletryMethods" {
        $telemtry = Invoke-GzWinTelemetry -IncludeLocalMembership -IncludeAdmins -IncludeBitLockerStatus -IncludeChrome -IncludePowershell
        
        $telemtry | Should Not Be $Null 

        $telemtry | ConvertTo-Json -Depth 10 | Out-File "$PSScriptRoot/test.json" -Encoding "UTF8"
    }
}