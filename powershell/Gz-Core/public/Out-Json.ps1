

function Out-Json () {
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Object] $InputObject,

        [Alias("o", "dest")]
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Destination,

        [int32] $Depth = 10,

        [Switch] $Compress,

        [Switch] $Force 
    )

    Process {
        
        $Path = [IO.Path]::GetFullPath($Destination)

        if ($PSCmdlet.ShouldProcess("InputObject", "Writing JSON to $Path")) {
            $dir = Split-Path $Path 
            $createDirectory = $Force.ToBool()
            $directoryExists = [IO.Directory]::Exists($dir)
            if(!$createDirectory -and !$directoryExists) {
                throw [IO.DirectoryNotFoundException] $dir 
            }

            if(!$directoryExists) {
                New-Item $dir -ItemType Directory | Write-Debug 
            }
            
            $json = $null;
            if(!($InputObject -is [String])) {
                if($InputObject -is [array]) {
                    $json = ConvertTo-Json ([array]$InputObject) -Depth $Depth -Compress:$Compress
                } else {

                    $json =  ConvertTo-Json $InputObject -Depth $Depth -Compress:$Compress
                }
            } else {
                $json = [string] $InputObject
            }
            
            # defaults to UTF8 with no Byte Order Mark ("BOM")
            [IO.File]::WriteAllText($Path, $json)
        }
    }
}