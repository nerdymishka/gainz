
Import-Module "$PSScriptRoot/../Gz-Yaml.psd1" -Force


Describe "Gz-Yaml" {

    Context "ConvertFrom-Yaml" {
        
        It "Should convert from YAML to Objects" {
          
            $content = Get-Content "$PsScriptRoot\sample.yaml" -Raw 
            $data = $content | ConvertFrom-Yaml;
            $data | Should Not Be $null
            $data.string -is [string] | should Be $True;
            $data.int -is [int32]| should be $True; 
            $data.datetime -is [DateTime] | Should Be $True;
            $data.bool -is [Boolean] | Should Be $True;
            $data.person | Should Not Be $null;
            $data.person.firstName | Should Be "Bob";
            $data.list -is [Array] | Should Be $True;
        }
    }

    Context "ConvertTo-Yaml" {

        It "Should convert from Objects to YAML" {
            $data = [PSCustomObject]@{
                Name = "Bob"
                Day = 7
            }

            $content = $data | ConvertTo-Yaml 
            $content | Should Not Be $null 
            $content -match "Name:" | Should Be $True 
            $content -match "Day:" | Should Be $True;
        }
    }
} 