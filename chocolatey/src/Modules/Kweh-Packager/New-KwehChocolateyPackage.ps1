function New-KwehChocolateyPackage() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path, 

        [Parameter(Position = 1)]
        [String] $Version,
        
        [Parameter(Position = 2)]
        [String] $Destination
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path
    }

    $PAth = $Path.Replace("\", "/")

    $json = Get-Item "$Path/kweh.json" -EA SilentlyContinue

    $requiredFields = @("id", "version", "authors", "title", "owners", "description")

    if($json) {
        $config = Read-KwehJsonConfig ($json.FullName) -Version $Version 
        $config | ConvertTo-Json -Depth 10 | Out-File "$Path/tools/kweh.json" -Encoding "UTF8"
        
        $nuspecObj = Step-KwehTransformConfigToNuSpec $config -PackageDir $Path -Chocolatey 
        $broken = $false 
        foreach($field in $requiredFields) {
            $value = $nuspecObj.metadata.$field
            if($value -eq $null) {
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

    
        $nuspec =  ConvertTo-NuSpec $nuspecObj -Chocolatey
    }

    if($nuspec) {
        $package = Split-Path $Path -Leaf 
        $nuspec.Save("$Path/$package.nuspec")

        if(!$Destination) {
            choco pack "$Path/$package.nuspec"
        } else {
            choco pack "$Path/$package.nuspec" --out=$Destination
        }
        return
    } else {
        Write-Warning "kweh configuration file not found in $Path"
        return 
    }

    
}