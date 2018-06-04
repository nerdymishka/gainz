
Param(
    [Parameter(Position = 0)]
    [String] $ProjectName,

    [Parameter(Position = 1)]
    [String] $PackageName 
)

    $cwd = "$PsScriptRoot/..";




    $projects = Get-ChildItem "$cwd/**/*.csproj" -Recurse
    Write-Host $projects
    foreach($proj in $projects)
    {
        Write-Host "$($proj.Name) $projectName.csproj"
        if($proj.Name -match "$projectName.csproj")
        {
            dotnet add $proj.FullName package $PackageName
            return;
        }
    }
    

