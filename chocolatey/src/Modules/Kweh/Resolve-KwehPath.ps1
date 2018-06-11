function Resolve-KwehPath {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PSCustomObject] $Context,

        [String] $BasePath
    )

    if($Path.StartsWith("./")) {
        $path = $path.Substring(2)
        if(![string]::IsNullOrWhiteSpace($BasePath)) {
            $path = "$BasePath/$path"
        } else {
            $destination = $Context.Destination
            $path = "$destination/$path" 
        }
    }

    if($Context -and $Path.Contains("{{")) {
        return Resolve-KwehStringTemplate -Template $Path -Model $Context 
    }

    return $Path;
}