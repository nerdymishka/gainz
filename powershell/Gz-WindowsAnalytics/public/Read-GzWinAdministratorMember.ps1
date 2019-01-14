
function Read-GzWinAdministratorMember() {

    return Get-GzWinLocalGroupMember -Group "Administrators"
}