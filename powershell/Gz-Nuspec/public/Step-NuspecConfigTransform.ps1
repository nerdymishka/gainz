function Step-NuspecConfigTransform() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [PsCustomObject] $Config, 

        [String] $PackageDir,

        [Switch] $Chocolatey 
    )

    write-host $Config.metadata

    $meta = @(
        "id", 
        "version", 
        "title",
        "authors",
        "owners",
        "licenseUrl",
        "requireLicenseAcceptance",     #boolean
        "developmentDependency"         #boolean
        "summary",
        "copyright",
        "language",
        "tags",
        "serviceable",                   #boolean
        "packageTypes",
        "dependencies",
        "frameworkAssemblies",
        "references",
        "releaseNotes",
        "description"
     )
     $root = @(
        "contentFiles",
        "files"
     )

     $arrays = @("tags", "authors", "owners")
     $markdown = @("description", "releaseNotes")

     # Chocolatey First Wave https://github.com/chocolatey/choco/issues/205
     $chocolateyProperties = @(
          "projectSourceUrl", 
          "packageSourceUrl",
          "docsUrl",
          "mailingListUrl",
          "bugTrackerUrl",
          "replaces",
          "provides",
          "conflits"
     )

     $nuspec = New-Object PsCustomObject -Property @{
         metadata = New-Object PsCustomObject 
     }
     
     $metadata = $Config.metadata;
     foreach($property in $meta) {
        if($null -ne $metadata.$property) {
            $value = $metadata.$property
            if($arrays.Contains($property)) {
                $value = [String]::Join(",", $value)
            }
            if($markdown.Contains($property)) {
                if($value.StartsWith("./")) {
                    $value = $value.SubString(2)
                    $value = ($PackageDir) + "/$value"
                }
                Write-Host $property
                Write-Host $value 
                if(Test-Path $value) {
                    Write-Host $value 
                    $value = Get-Content $value -Raw
                } else {
                    if($value.StartsWith("http")) {
                        $value = "[$Property]($value)"
                    }
                }
            }

            $nuspec.Metadata | Add-Member NoteProperty -Name $property -Value $value -Force  
        }
     }

     if(!$nuspec.metadata.title) {
        $nuspec.metadata | Add-Member NoteProperty -Name "title" -Value ($metadata.id) -Force
     }

     if($Chocolatey.ToBool()) {
         foreach($property in $chocolateyProperties) {
            if($null -ne $Config.$property) {
                $value = $Config.$property

                if($arrays.Contains($property)) {
                    if($property -eq "tags") {
                        $value = [String]::Join(" ", $value)
                    } else {
                        $value = [String]::Join(",", $value)
                    }
                }
                if($markdown.Contains($property)) {
                    if($value.StartsWith("./")) {
                        $value = $value.SubString(2)
                        $value = "$PackageDir/$value"
                    }
                    if(Test-Path $value) {
                        $value = Get-Content $value -Raw
                    } else {
                        if($value.StartsWith("http")) {
                            $value = "[$Property]($value)"
                        }
                    }
                }

                $nuspec.Metadata | Add-Member NoteProperty $property $value 
            }
         }
     }

    foreach($property in $root) {
        if($null -ne $Config.$property) {
            $value = $Config.$property 

            $nuspec | Add-Member NoteProperty $property $value 
        }
    }

    return $nuspec
}