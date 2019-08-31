
Import-Module "$PsScriptRoot/../../Gz-Core/*.psm1" -Force
Import-Module "$PsScriptRoot/../*.psd1" -Force

InModuleScope "Gz-VisualStudio" {

    Describe "Get-ModuleVariable" {

        It "should return a hash object" {
            $vars = Get-ModuleVariable 
            $vars | Should Not Be $Null 
            $vars.GetType().Name | Should Be "Hashtable" 
        }
    }


    Describe "Get-VsWhereLocation" {

        It "should return the vsWhere path using Get-Command" {
            Mock Get-Command -MockWith { return @{Path = "/apps/vsWhere.exe" }}
            $vsWhere = Get-VsWhereLocation 
            $vsWhere | Should Not Be $Null 
            $vsWhere | Should Be "/apps/vsWhere.exe"
        }

        It "should return the vsWhere path using Get-Item" {
            Mock Get-Command -MockWith { return $null }
            Mock Get-Item -MockWith { return @{ FullName = "/apps/get-item/vsWhere.exe" } }
            Mock Test-Path -MockWith { return $true }
           
            $vsWhere = Get-VsWhereLocation 
            $vsWhere | Should Not Be $Null 
            $vsWhere | Should Be "/apps/get-item/vsWhere.exe"
        }       
    }

    Describe "Get-VisualStudioPath" {
        $vsWherePaths = @{
            "15.0" = "/vsWhere/15.0"
            "14.0" = "/vsWhere/14.0"
            "13.0" = "/vsWhere/13.0"
        }

        $registryPaths = [PsCustomObject]@{
            "15.0" = "/software/15.0"
            "14.0" = "/software/14.0"
            "13.0" = "/software/13.0"
            "junk" = "garbage-data"
        }

        It "should return all versions using vsWhere" {
            Mock Read-VsWherePathData { return $vsWherePaths }
            $paths = Get-VisualStudioPath -AsHashtable

            $paths | Should Not Be $null 
            $paths["15.0"] | Should Be "/vsWhere/15.0"

            $paths = Get-VisualStudioPath 
            $paths | Should Not Be $null 
            $paths[0].Path | Should Not Be $Null 
            $paths[0].Path.GetType().Name | Should Be "String"
        }

        It "should return latest versions using vsWhere" {
            Mock Read-VsWherePathData { return $vsWherePaths }
            $path = Get-VisualStudioPath -Latest

            $path | Should Not Be $Null 
            $path | Should Be "/vsWhere/15.0"
        }

        It "should return specific version using vsWhere" {
            Mock Read-VsWherePathData { return $vsWherePaths }
            $path = Get-VisualStudioPath "13.0"

            $path | Should Not Be $Null 
            $path | Should Be "/vsWhere/13.0"
        }

        It "should return all versions using the registry " {
            Mock Read-VsWherePathData { return $null }
            Mock Get-ItemProperty { return $registryPaths }

            $paths = Get-VisualStudioPath -AsHashtable -Force
            $paths | Should Not Be $null 
            $paths["15.0"] | Should Be "/software/15.0"

            $paths = Get-VisualStudioPath 
            $paths | Should Not Be $null 
            $paths[0].Path | Should Not Be $Null 
            $paths[0].Path.GetType().Name | Should Be "String"
        }


        It "should return latest versions using vsWhere" {
            Mock Read-VsWherePathData { return $null }
            Mock Get-ItemProperty { return $registryPaths }

            $path = Get-VisualStudioPath -Latest

            $path | Should Not Be $Null 
            $path | Should Be "/software/15.0"
        }

        It "should return specific version using vsWhere" {
            Mock Read-VsWherePathData { return $null }
            Mock Get-ItemProperty { return $registryPaths }

            $path = Get-VisualStudioPath "13.0"

            $path | Should Not Be $Null 
            $path | Should Be "/software/13.0"
        }
    }



}