
Properties {
    $VersionSuffix = if($ENV:GAINZ_VERSION_SUFFIX) { $ENV:GAINZ_VERSION_SUFFIX } else { "" }
    $BuildConfiguration = "Release"
    $NugetFeed = $Env:GAINZ_DOTNET_NUGET_FEED
    $NugetApiKey = $Env:GAINZ_DOTNET_NUGET_API_KEY
    $ArtifactsPath = "$PsScriptRoot/.tmp/artifacts"
}

Task "Restore" {
    exec {
        dotnet.exe restore ./Gainz.sln
    }
}

Task "Build" -depends "restore"  {
    exec {dotnet build ./Gainz.sln -c "$BuildConfiguration" }
}

Task "Test" {
   
    exec {
        $projects = Get-Item "$PsScriptRoot\test\**\*Tests.csproj"

        foreach($project in $projects)
        {
            dotnet test $project 
        }
    }
}

 Task "Clean" {
    Remove-Item "$PsScriptRoot/.tmp" -Recurse -Force
    exec { & dotnet.exe clean ./Gainz.sln } 
 }


Task "Pack"  {
    $skip = @("NerdyMishka.Flex.Proto.csproj", "NerdyMishka.Lucene.Core.csproj")
    $projects = Get-Item "$PsScriptRoot\src\**\*.csproj"
    $packagesDir = "$ArtifactsPath\packages"
    $packagesOld = "$ArtifactsPath\packages-old"

    if(Test-Path $packagesDir)
    {
    
        if(!(Test-PAth $packagesOld))
        {
            New-Item $packagesOld -ItemType Directory 
        }
        
        $items = Get-Item "$packagesDir/*.*"
        foreach($i in $items)
        {
            Write-Host $i
            Move-Item $i $packagesOld -Force
        }
       
        Remove-Item "$packagesDir" -Force -Recurse
    } 
    New-Item $packagesDir -ItemType Directory -Force 
   

    $c = $BuildConfiguration;
    if([string]::IsNullOrWhiteSpace($c)) {
        $c = "Release"
    }

    $versions = Get-Content "$PsScriptRoot/build/versions.json" -Raw | ConvertFrom-Json 
    Write-Host $versions;
    foreach($project in $projects)
    {
        $name = [System.IO.Path]::GetFileNameWithoutExtension($project)
        if([string]::IsNullOrEmpty($versions.$name)) 
        {
            Write-Warning "Version Info Missing: $name"
            continue;
        }

        $version = $versions.$name;
        if($version -eq $false) {
            continue;
        }
        if($VersionSuffix){
            $version += "-$VersionSuffix"
        }

        exec { & dotnet pack $project.FullName -o "$packagesDir" -c "$c" /p:Version=$Version }
    }

    
}


Task "Publish" {
    $packagesDir = "$ArtifactsDir\packages"
    $packages = Get-ChildItem "$packagesDir\*.nupkg"
    foreach($package in $packages)
    {
        exec { dotnet nuget push $package -k $NugetApiKey -s $NugetFeed } 
    }
}

