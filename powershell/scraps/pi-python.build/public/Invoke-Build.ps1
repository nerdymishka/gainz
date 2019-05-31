

function Invoke-Build() {
    Param(
        [String] $Path,
        [Switch] $x64
    )



    $platform = "win32";
    $suffix = "";

    if($x64.ToBool()) {
        $platform = "amd64";
        $suffix = "-x64"
    }

    
    $root = $Path;
    $o = "$root/build";
    if(!(test-path "$o")) {
        mkdir "$o/libs"
        mkdir "$o/Lib"
        mkdir "$o/DLLs"
        mkdir "$o/tools"
    }
    $extensions = @(
        'pyexpat',
        'select',
        'unicodedata',
        'winsound',
        "_bz2",
        '_elementtree',
        '_socket',
        '_ssl',
        '_msi',
        '_ctypes',
        '_hashlib',
        '_multiprocessing',
        '_lzma',
        '_decimal',
        '_overlapped',
        '_sqlite3',
        '_asyncio',
        '_queue',
        '_distutils_findvs',
        '_contextvars'
    )

    $libDlls = @(
        "libssl-1_1${suffix}.dll",
        "libcrypto-1_1${suffix}.dll",
        'sqlite3.dll'
    )

    $main = @(
        'python.exe',
        'python3.dll',
        'python37.dll',
        'pythonw.exe'
    )

    $tools = @(
        'demo',
        'i18n',
        'parser',
        'pynche',
        'scripts'
    )

    Copy-Item "$root/Include/**" "$o/include" -Force -Recurse


    foreach($lib in $extensions) {
        if(Test-Path "$root/PCBuild/$platform/$lib.lib") {
            Copy-Item  "$root/PCBuild/$platform/$lib.lib"  "$o/libs" -Force
        }

        if(Test-Path "$root/PCBuild/$platform/$lib.pyd") {
            Copy-Item  "$root/PCBuild/$platform/$lib.pyd"  "$o/DLLs" -Force
        }
    }

    foreach($lib in $libDlls) {
        if(Test-Path "$root/PCBuild/$platform/$lib") {
            Copy-Item  "$root/PCBuild/$platform/$lib"  "$o/DLLs" -Force
        }
    }

    foreach($file in $main) {
        if(Test-Path "$root/PCBuild/$platform/$file") {
            Copy-Item  "$root/PCBuild/$platform/$file"  "$o" -Force
        }
    }



    $set = @();
    $excludedDir = @('test', 'tests', 'tkinter', 'idlelib', 'turtledemo')
    $children = gci "$Root/Lib"

    foreach($child in $children) {
        if($excludedDir.Contains($child.Name)) {
            continue;
        }
        $set += $child;
    }

    foreach($file in $set) {
        if($file -is  [System.IO.DirectoryInfo]) {
            Copy-Item $file.FullName "$o/Lib" -Exclude "*.pyc", "*.pyo" -Recurse -Force
            continue;
        }

        if($file.Name.EndsWith(".pyc") -or $file.Name.EndsWith(".pyo")) {
            continue;
        }

        Copy-Item $file.FullName "$o/Lib" -Force
    }

    


    foreach($tool in $tools) {
        Copy-Item "$root/Tools/$tool" "$o/tools" -Force
    }
}




exit;
