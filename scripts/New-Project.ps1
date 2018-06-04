[CmdletBinding()]
Param(
    [Parameter(Position = 0)]
    [String] $ProjectName,

    [String] $Type = "classlib",

    [String] $Language = "C#",

    [String] $Sln = "Gainz.sln"
)

$cwd = "$PSScriptRoot/../";

$dir = $ProjectName.Replace("Gainz.", "").Replace("NerdyMishka.", "");

switch($type) {
    "sln" {
        dotnet new "sln" -n $ProjectName -o "$cwd"
    }
    "mstest" {
        if(-not (Test-Path "$cwd/test")) {
            New-Item "$cwd/test" -ItemType Directory
        }
        dotnet new "mstest" -lang $Language -n $ProjectName -o "$cwd/test/$dir"
        dotnet sln $Sln add "$cwd/test/$ProjectName.csproj"
    }
    "xunit" {
        if(-not (Test-Path "$cwd/test")) {
            New-Item "$cwd/test" -ItemType Directory
        }
        dotnet new "xunit" -lang $Language -n $ProjectName -o "$cwd/test/$dir"
        dotnet sln $Sln add "$cwd/test/$ProjectName.csproj"
    }
    default {
        if(-not (Test-Path "$cwd/src")) {
            New-Item "$cwd/src" -ItemType Directory
        }

        & dotnet new $type -lang $Language -n $ProjectName -o "$cwd/src/$dir"
        & dotnet sln "$cwd/$Sln" add "$cwd/src/$dir/$ProjectName.csproj"
    }
}
