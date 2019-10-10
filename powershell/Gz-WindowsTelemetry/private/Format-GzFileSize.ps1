

if($null -eq (Get-Command Format-FileSize -EA SilentlyContinue))
{
    function Format-FileSize {
    
        Param(
            [PArameter()]
            [double] $Length,
    
            [Parameter(ValueFromPipeline= $true, Position = 0)]
            [System.IO.FileInfo] $File 
        )
    
        if($File) {
            $Length = $File.Length
        }
    
        $sizes = @("B", "KB", "MB", "GB", "TB", "PB" );
    
        $order = 0;
        while ($Length -ge 1024 -and $order -lt $sizes.Length - 1) {
            $order++;
            $Length = $Length / 1024;
        }
    
    
        return [String]::Format("{0:0.##} {1}", $Length, $sizes[$order]);
    }

}
