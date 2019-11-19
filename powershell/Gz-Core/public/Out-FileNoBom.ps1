

function Out-FileNoBom () {
    [CmdletBinding(SupportsShouldProcess)]
    Param(
        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Object] $InputObject,

        [Alias("o", "dest")]
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
        [String] $Destination,

        [Switch] $Append,

        [Switch] $NoClobber,

        [Switch] $Force 
    )

    Process {
        
        $Path = [IO.Path]::GetFullPath($Destination)
        $dir = Split-Path $Path 
        if(!(Test-Path $dir)) {
            if($Force.ToBool()) {
                New-Item -ItemType Directory $dir 
            } else {
                throw [System.IO.DirectoryNotFoundException] $dir 
            }
        }


        $content = $InputObject.ToString()

        if ($PSCmdlet.ShouldProcess("InputObject", "Writing JSON to $Path")) {
           
            if($Append.ToBool()) {
                [IO.File]::AppendAllText($Path, $content)
                return;
            }

            if($NoClobber.ToBool() -and (Test-Path $Destination)) {
                throw [System.InvalidOperationException] "The file already exists and must not be overwritten: $Destination "
            }
            
            # defaults to UTF8 with no Byte Order Mark ("BOM")
            [IO.File]::WriteAllText($Path, $content)
        }
    }
}