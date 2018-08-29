

function Add-DbProviderFactory() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name,

        [Parameter(Position = 1)]
        [System.Data.Common.DbProviderFactory] $Factory,

        [Switch] $Default 
    )

    Set-SqlDbOption -Name "DbProviderFactories/$Name" -Value $Factory

    if($Default.ToBool()) {
        Set-SqlDbOption -Name "DbProviderFactories/Default" -Value $Factory
    }
}