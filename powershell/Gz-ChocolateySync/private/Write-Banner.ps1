$bannerWriteOnce = $false;

function Write-Banner() {
    Param(
        [Switch] $Force 
    )
    if($bannerWriteOnce -or !$Force.ToBool()) {
       return;
    }

 
Write-Host "
    _     _                                                                     
   (c).-.(c)         _   _              _         __  __ _     _     _          
    / ._. \         | \ | | ___ _ __ __| |_   _  |  \/  (_)___| |__ | | ____ _  
  __\( Y )/__       |  \| |/ _ \ '__/ _`` | | | | | |\/| | / __| '_ \| |/ / _`` | 
 (_.-/'-'\-._)      | |\  |  __/ | | (_| | |_| | | |  | | \__ \ | | |   < (_| | 
    || M ||         |_| \_|\___|_|  \__,_|\__, | |_|  |_|_|___/_| |_|_|\_\__,_| 
  _.' ``-' '._                             |___/                                 
 (.-./``-'\.-.)                                                                  
  ``-'     ``-'        Gainz: Chocolatey Package Sync                           
                                                                                " `
    -ForegroundColor DarkRed -BackgroundColor Black
}
Write-Banner