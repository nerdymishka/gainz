
function New-LinuxGroup() {
    Param(
        [String] $Name,
        
        [Int] $Id = -1
    )

    While([string]::IsNullOrWhiteSpace($Name)) {
        $Name = Read-Host -Prompt "Name"
    }
    $splat = @();
    if($Id -gt -1) {
        $splat += "-g"
        $splat += "$Id"
    }

    group @splat

    if($LASTEXITCODE -eq 0) {
        return [PSCustomObject]@{
            Name = $Name
            Id = $Id 
        }
    }
    
    return $null;
}