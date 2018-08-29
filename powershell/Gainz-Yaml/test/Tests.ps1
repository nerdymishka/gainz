
if($null -eq  [Type]::GetType("YamlDotNet.RepresentationModel.YamlScalarNode")) {
    if($PSVersionTable.PSEdition -eq "Core") {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\..\bin\netstandard1.3\YamlDotNet.dll") | Out-Null
    } else {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\..\bin\net45\YamlDotNet.dll") | Out-Null
    }
}

Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Describe "Gainz-Yaml" {

    Context "ConvertFrom-Yaml" {

        It "should convert from yaml to powershell objects" {
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

        It "should convert from powershel to yaml" {
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