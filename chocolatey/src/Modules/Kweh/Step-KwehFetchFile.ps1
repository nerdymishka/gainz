function Step-KwehFetchFile() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Instructions,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context  
    )

    $alg = if($Installer.alogrithm) { $Installer.alogrithm } else { "SHA256" }

    if($Context.Is64Bit -and $Instructions.x64) {
        $uri = [URI] ($Context | Resolve-StringTemplate ($Instructions.x64.uri));
        $hash = $Instructions.x64.hash;
        if($Instructions.x64.alogrithm) {
            $alg = $Instructions.x64.alogrithm
        }
    }  else  {
        $uri = [URI] ($Context | Resolve-StringTemplate ($Instructions.uri));
        $hash = $Instructions.hash;
    }

    

    $installLabel = $Context.InstallLabel 
    $tmp = $Context.Tmp 

    if([string]::IsNullOrWhiteSpace($tmp)) {
        Write-Warning "Context.Tmp has no value."
        return
    }

    $dest = "$tmp/$installLabel"

    if((Test-Path $dest)) {
        Remove-Item $dest -Recurse -Force | Write-Debug
    }

    New-Item $dest -ItemType Directory -Force | Write-Debug

    $package = $null

    
    if($uri.IsFile) {
        Copy-Item $uri "$dest"

        $fileName = [System.IO.Path]::GetFileName($uri.ToString())

        $package = "$dest/$fileName"
        if(! (Test-PAth $package)) {
            throw System.IO.FileNotFoundException $package 
        }

        $package = gi $package
    } else {
        

        $package = Invoke-KwehWebRequest -Uri $uri -OutFile "$dest"
    }

    $continue = Step-ValidateChecksum -Path ($Package.FullName) -Hash $hash -Algorithm $alg 
    if($continue) {
        $destination = $Context.Destination
        if(!$destination) {
            $opt = $Context.Opt 
            $installLabel = $Context.InstallLabel

            $destination = "$opt/$installLabel"
        }
        $name = $package.Name 
        Copy-Item ($Package.FullName) $destination 

        $Context | Add-Member NoteProperty -Name Package -Value "$destination/$name"
        return $true
    }

    Write-Warning "Checksum failed. Hashes do not match";
    return $false;
}
Set-Alias -Name Set-FetchFile -Value Step-KwehFetchFile  