Import-Module "$PSScriptRoot/../Gz-Checkpoint.psm1" -Force

Describe "Gz-Checkpoint" {

    IT "Should add a named stored" {
        Add-GzCheckpointStore -Name "test" -Path "$PsScriptRoot/test.json"

        Test-Path "$PsScriptRoot/test.json" | Should Not Be $False
    }

    It "Should save to a named store" {
        Save-GzCheckpoint "check-check" -Store "test"

        $data = Get-Content "$PsScriptRoot/test.json" -Raw | ConvertFrom-Json 
        $hash = @{}
        $data | Get-Member -MemberType NoteProperty | ForEach-Object {
            $n = $_.Name 
            $v = $data.$n
            $hash[$n] = $v
        }
        $hash["__gz"] | Should Be $True 
        $hash["check-check"] | Should Be $True 
    }

    It "Should test a checkpoint" {
        Test-GzCheckpoint "check-check" -Store "test" | Should Be $True 
    }

    Remove-Item "$PsScriptRoot/test.json" -Force
}