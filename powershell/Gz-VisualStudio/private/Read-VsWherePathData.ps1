
$vsProduct = @{
    "Microsoft.VisualStudio.Product.Explorer" = -1
    "Microsoft.VisualStudio.Product.TestAgent"=  0
    "Microsoft.VisualStudio.Product.Community" = 1
    "Microsoft.VisualStudio.Product.BuildTools" = 2
    "Microsoft.VisualStudio.Product.Professional" = 3
    "Microsoft.VisualStudio.Product.Enterprise" = 4
}



function Read-VsWherePathData()
{
    $vsWhere = Get-VsWhereLocation
    if(!$vsWhere) { return @{} }

    $vsPaths = @{}
    $vsVersionInfo = @{};

    $buildVersions = & $vsWhere -format json -products 'Microsoft.VisualStudio.Product.BuildTools'
    $vsVersions = & $vsWhere -format json 
    $versions = @();
    $buildVersions = $buildVersions | ConvertFrom-Json
    $vsVersions = $vsVersions  | ConvertFrom-Json 

    foreach($v in $buildVersions) {
        $versions += $v;
    }

    foreach($v in $vsVersions) {
        $versions += $v;
    }

    if($versions -and $versions.Length) {
        foreach($v in $versions) {
            if($v.installationVersion) {
                $major = $v.installationVersion.ToString()
                $major = $major.Substring(0, $major.IndexOf("."))
                $entry = "$major.0";
                $default = $null 
                if($vsVersionInfo.ContainsKey($entry) -and !($v.isPrerelease)) {
                    $default = $vsVersionInfo[$entry]
                    $scale = $vsProduct[$v.productId]
                    if($scale -gt $default) {
                        $vsVersionInfo[$entry] = $scale;
                        $vsPaths[$entry] = $v.installationPath;
                    }
                } else {
                    $vsVersionInfo[$entry] = $vsProduct[$v.productId]
                    $vsPaths.Add($entry, $v.installationPath)
                }
                
                switch($v.productId) {
                    "Microsoft.VisualStudio.Product.TestAgent" {
                        continue;
                    }
                    "Microsoft.VisualStudio.Product.Explorer" {
                        continue;
                    }
                    "Microsoft.VisualStudio.Product.Community" {
                        $entry = "community:$major.0"
                    }
                    "Microsoft.VisualStudio.Product.Professional" {
                        $entry = "professional:$major.0"
                    }
                    "Microsoft.VisualStudio.Product.Enterprise" {
                        $entry = "enterprise:$major.0"
                    }
                    "Microsoft.VisualStudio.Product.BuildTools" {
                        $entry = "buildtools:$major.0" 
                    }
                }
                
              
                if($v.isPrerelease) {
                    $entry += "-pre"
                }
                $vsPaths.Add($entry, $v.installationPath);
            }
        }
    }

    return $vsPaths
}