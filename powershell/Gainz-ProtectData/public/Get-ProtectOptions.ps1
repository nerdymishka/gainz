$protectOptions = $null

function Set-ProtectOptions() {
    Param(
        [PsCustomObject] $options
    )

    $protectOptions = $options;
}

function Get-ProtectOptions() {
    
    if($protectOptions) {
        return $protectOptions
    }

  
    $protectOptions = New-Object PsCustomObject -Property @{
        BlockSize = 128
        KeySize = 256
        SaltSize = 64
        Iterations = 10000
        MiniumPasswordLength = 12
    }

    return $protectOptions
}
