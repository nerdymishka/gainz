function Uninstall-KwehPackage() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path
    )

    if([string]::IsNullOrWhiteSpace($Path)) {
        throw [System.ArgumentException] "Path must have a value."
    }

    if(!$Path.EndsWith(".json")) {
        throw [System.ArgumentException] "Path must be a json file."
    }

    if(! (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] "$Path"
    }

    $config = Read-KwehConfig $Path 
    $context = Step-BuildContext $config 

    $context | Step-BeforeUninstall $config 
    $context | Step-Uninstall $config 
    $context | Step-AfterUninstall $config 
}