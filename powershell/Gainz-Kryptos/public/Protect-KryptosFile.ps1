

function Protect-KryptosFile()
{
    Param(
        [Alias("s")]
        [Parameter(Position = 0)]
        [String] $Source,

        [Alias("o")]
        [Parameter(Position = 1)]
        [String] $Destination,

        [Alias("f")]
        [Switch] $Force,

        [Alias("k")]
        [Uri] $Key
    )

    $src = $Source
    $dest = $Destination
    if([string]::IsNullOrEmpty($Src)) {
        if($PSEdition -ne "Core" -and [System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Win32NT) {
            while([string]::IsNullOrWhiteSpace($src)) {
                Add-Type -AssemblyName System.Windows.Forms
                $SaveChooser = New-Object -Typename System.Windows.Forms.SaveFileDialog
                $SaveChooser.ShowDialog()
                $src = $SaveChooser.FileName;

                if([string]::IsNullOrWhiteSpace($src) -or  !(Test-Path $src))
                {
                    $src = $null
                }
            }
        } else {
            while([string]::IsNullOrWhiteSpace($src)) {
                $src = Read-Host -Prompt "file to encrypt";

                if([string]::IsNullOrWhiteSpace($src) -or  !(Test-Path $src))
                {
                    $src = $null
                }
            }
        }
    }

    if([string]::IsNullOrEmpty($dest)) {
        $dest = $src;
    }

    if(!$dest.EndsWith(".krypt")) {
        $dest += ".krypt";
    }

    $splat = @('-s', "`"$src`"", '-o', "`"$dest`"");

    if($Force.ToBool()) {
        $splat += '-f';
    }

    if($Key) {

        if($key.IsFile) {
            $path = $key.LocalPath
            if(!(Test-Path $path)) {
                Write-Warning "Could not find public key at $path"
                return;
            }

            $splat += '-k'
            $splat += "`"$path`""
        } else {
            $splat += '-k'
            $splat += "`"$($Key.ToString())`""
        }
    }

    & kryptos file encrypt @splat
}