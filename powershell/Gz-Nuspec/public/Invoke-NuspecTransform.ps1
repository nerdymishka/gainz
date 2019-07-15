function Invoke-NuspecTransform() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path, 

        [Parameter()]
        [Switch] $Chocolatey,

        [Parameter(Position = 1)]
        [String] $Version,
        
        [Parameter(Position = 2)]
        [String] $Destination
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path
    }

    $Path = $Path.Replace("\", "/")
    $dir = $null;
    $file = Get-Item $Path 

    if($file -is [System.IO.DirectoryInfo]) {
        $dir = $Path;
        $possibilities = @("$Path/nuspec.json", "$Path/nuspec.yaml", "$Path/nuspec.yml")

        foreach($test in $possibilities)
        {
            if(Test-path $test) {
                $file = Get-ITem $test;
                break;
            }
        }
    } else {
        $dir = $file.FullName | Split-Path
    }

    
    $requiredFields = @("id", "version", "authors", "title", "description") 
    $config = Read-NuspecConfig -PAth $file.FullName -Version $Version 

    # json is always available in powershell 3+

    if(!(Test-Path "$dir/tools")) {
        new-item "$dir/tools" -ItemType Directory
    }

    $config | ConvertTo-Json -Depth 10 | Out-File "$dir/tools/nuspec.json" -Encoding "UTF8"
    
    $nuspecObj = Step-NuspecConfigTransform $config -PackageDir $Path -Chocolatey:$Chocolatey
    write-Host $nuspecObj.metadata
    $broken = $false 
    foreach($field in $requiredFields) {
        $value = $nuspecObj.metadata.$field
        if($null -eq $value) {
            Write-Error "kweh config is missing required field $field"
            $broken = $true; 
        }
        if($field -eq "authors" -or $field -eq "owners") {
            
            if($value.Length -eq 0) {
                Write-Error "$field must have at least one entry in the array"
                $broken = $true;
            }
        }
    }

    if($broken) {
        return;
    }


    $nuspec =  ConvertTo-NuSpec $nuspecObj -Chocolatey -RelativePath $dir
    

    if($nuspec) {
        if([string]::IsNullOrWhiteSpace($Destination)) {
            $package = Split-Path $dir -Leaf 
            $Destination = "$dir/$package.nuspec"
        }

        

     
        $nuspec.Save($Destination)
    } else {
        Write-Warning "kweh configuration file not found in $Path"
        return 
    }
}