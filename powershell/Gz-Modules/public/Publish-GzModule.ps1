function Publish-GzModule() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Path,

        [Parameter(Position = 1)]
        [String] $NugetApiKey,

        [String] $StagingDirectory,

        [String] $Respository,

        [String[]] $Include,

        [pscredential] $Credential,

        [Switch] $WhatIf,

        [string[]] $Tags = $null,

        [String] $ReleaseNotes,

        [String] $LicenseUri,

        [String] $IconUri,

        [String] $ProjectUri,

        [version] $RequiredVersion,

        [version] $FormatVersion,

        [switch] $Force
    )

    if([String]::IsNullOrWhiteSpace($StagingDirectory)) {
        $StagingDirectory = Get-GzPublishArtifactsDirectory
        if(!(Test-Path $StagingDirectory)) {
            New-Item -ItemType Directory $StagingDirectory
        }
    }

    if([string]::IsNullOrWhiteSpace($Respository)) {
        $Respository = "PSGallery"
    }

    $Path = (Resolve-Path $Path).Path
    if(-not (Test-Path $Path)) {
        Write-Warning "Could not locate $Path";
        return;
    }

    $ogStaging = $StagingDirectory
    $name = Split-Path $Path -Leaf;
    $StagingDirectory = "$StagingDirectory/$Name"
   

    if(Test-Path "$StagingDirectory") {
        Remove-Item "$StagingDirectory" -Force -Recurse
    }

    New-Item "$StagingDirectory" -ItemType Directory

    $dirs = @("public", "private", "lib", "bin");
    $files = @("$Name.psm1", "$Name.psd1", "*.ps1", "LICENSE", "LICENSE.md", "LICENSE.txt", "README.md", "README.txt")

    if($Include -and $Include.Length) {
        foreach($f in $Include) {
            $files += $f 
        }
    }
  

    foreach($dir in $dirs)
    {
        $t = Get-Item "$Path/$dir" -ErrorAction SilentlyContinue
        if($t) {
            Write-Host $t.Name;
            Copy-Item -Path $t.FullName -Destination "$StagingDirectory/$dir" -Recurse 
        }
    }

    foreach($file in $files)
    {
        $t = Get-Item "$Path/$file" -ErrorAction SilentlyContinue
        if($t -is [Array])
        {
            Copy-Item $t -Destination "$StagingDirectory"
            continue;
        }
        if($t) {
           
            Copy-Item -Path $t.FullName -Destination "$StagingDirectory/$file"  
        }
    }
   
    $args = @{
       "NugetApiKey" = $NugetApiKey
       "Path" = $StagingDirectory
       "WhatIf" = $WhatIf.ToBool()
       "Credential" = $Credential
    }

    if(![string]::IsNullOrWhiteSpace($Respository)) {
        $args.Add("Repository", $Respository)
    }

    if($null -ne $Tags -and $Tags.Length) {
        $args.Add("Tags", $Tags)
    }

    if($RequiredVersion) {
        $args.Add("RequiredVersion", $RequiredVersion)
    }

    if($FormatVersion) {
        $args.Add("FormatVersion", $FormatVersion)
    }
    

    if(![string]::IsNullOrWhiteSpace($ReleaseNotes)) {
        $args.Add("ReleaseNotes", $ReleaseNotes)
    } else {
        $files = @("$Path/RELEASENOTES.md", "$PATH/RELEASENOTES.txt")
        foreach($f in $files) {
            if(Test-Path $f) {
                $args.Add("ReleaseNotes", (Get-Content $f -Raw))
                break;
            }
        }
    }

    if(![string]::IsNullOrWhiteSpace($IconUri)) {
        $args.Add("IconUri", $IconUri)
    }

    if(![string]::IsNullOrWhiteSpace($ProjectUri)) {
        $args.Add("ProjectUri", $ProjectUri)
    }

    if(![string]::IsNullOrWhiteSpace($LicenseUri)) {
        $args.Add("LicenseUri", $LicenseUri)
    }

    if(Test-Path "$Path/dependences.xml") {
        $deps = [xml](Get-Content "$Path/dependencies.xml" -Raw)
        $nugetExe = Get-Command nuget.exe -EA SilentlyContinue 
        if(!$nugetExe) {
            throw "Could not locate nuget.exe on path"
        }

        $nugetExe = $nugetExe.Path
        
        # this will register GaArtifacts, only if it does not exist
        # or if the paths to not match.
        Register-GzArtifactsRepository 
        
        $ogStaging = Get-GzPublishArtifactsDirectory
        $tmpDir = "$ogStaging/tmp"
        $feedDir = "$ogStaging/feed"


        if(!(Test-Path $tmpDir)) {
            New-Item -ItemType Directory $tmpDir 
        }

        if(Test-Path "$tmpDir/$name.zip") {
            Remove-Item "$tmpDir/$name.zip"
        }

        if(Test-Path "$tmpDir/$name") {
            Remove-Item "$tmpDir/$name" -Recurse -Force
        }

       

        $args["Repository"] = "GzArtifacts"
        if($args.ContainsKey("WhatIf")) {
            $args.Remove("WhatIf");
        }

        # publish to local folder
        Publish-Module @args 

        $pkg =  (Get-Item "$feedDir/$name*.nupkg").FullName
        $contentsDir = "$tmpDir/$name"
        Move-Item $pkg "$tmpDir/name.zip"
        Expand-Archive "$tmpDir/$name.zip" $contentsDir 

        $relDir = Join-Path $contentsDir -ChildPath "_rels"
        $contentPath = Join-Path $contentsDir -ChildPath '`[Content_Types`].xml'
        $packPath = Join-Path $contentsDir -ChildPath "package"
        $modulePath = Join-Path $contentsDir -ChildPath ($ModuleName + ".nuspec")

        #<package xmlns="http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd">

        # update the nuspec file to include the dependencies
        $nuspec = [xml](Get-Content $modulePath -Raw)

        $ns = New-Object System.Xml.XmlNamespaceManager($nuspec.NameTable)
        $xmlns = $nuspec.DocumentElement.NamespaceURI
        $ns.AddNamespace("ns", $xmlns) | Out-Null
        $metadataElement = $nuspec.SelectSingleNode("//ns:metadata", $ns)

        $dependenciesElement = $nuspec.CreateElement("dependencies", $xmlns);
        foreach($child in $deps.ChildNodes) {
            $dependency = $nuspec.CreateElement("dependency", $xmlns);
            $dependency.SetAttribute("id", $child.Attributes["id"].Value)
            $dependency.SetAttribute("version",  $child.Attributes["version"].Value)
            
            $dependenciesElement.AppendChild($dependency)
        }

        $metadataElement.AppendChild($dependenciesElement);

        $nuspec.Save($modulePath)
        

        # Cleanup
        Remove-Item -Recurse -Path $relDir -Force
        Remove-Item -Recurse -Path $packPath -Force
        Remove-Item -Path $contentPath -Force

        

        # https://stackoverflow.com/a/36369540/294804
        &$NugetExe pack $modulePath -OutputDirectory $feedDir -NoPackageAnalysis

        $repo = Get-PSRepository -Name $Respository
        

        if($WhatIf.ToBool()) {
            Write-Debug "& $NugetExe push $pkg [ApiKey] -s $($repo.PublishLocation)"
        } else {
            &$NugetExe push $pkg $NugetApiKey -s $repo.PublishLocation 
        }

        Remove-Item $pkg 
        Remove-item "$tmpDir/$name.zip"
        Remove-Item "$tempDir/$name" -Recurse -Force
        return  
    }

    return Publish-Module @args
}