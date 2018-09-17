$bannerWriteOnce = $false;

function Write-Banner() {
    Param(
        [String] $Message,
        [Switch] $WriteHost 
    )

 
$banner = "
    _     _                                                                     
   (c).-.(c)         _   _              _         __  __ _     _     _          
    / ._. \         | \ | | ___ _ __ __| |_   _  |  \/  (_)___| |__ | | ____ _  
  __\( Y )/__       |  \| |/ _ \ '__/ _`` | | | | | |\/| | / __| '_ \| |/ / _`` | 
 (_.-/'-'\-._)      | |\  |  __/ | | (_| | |_| | | |  | | \__ \ | | |   < (_| | 
    || M ||         |_| \_|\___|_|  \__,_|\__, | |_|  |_|_|___/_| |_|_|\_\__,_| 
  _.' ``-' '._                             |___/                                 
 (.-./``-'\.-.)                                                                  
  ``-'     ``-'        $Message                            
                                                                                " `
    if($WriteHost.ToBool()) {
        $interactive = Test-Interactive
        if($interactive) {
            Write-Host $banner -Foreground "DarkRed" -Background "Black"
        }
    } else {
        return $banner;
    }
}
