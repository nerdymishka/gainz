# Gainz PasswordGenerator

Generates a pasword using .NET's csrng method. The characters used to generate the password, the validatation function and the
length of the password is customizable.

Since strings are immutable, the function will return an array of characters by default. The `-AsSecureString` will return secure
string and `-AsString` will return a string.

```powershell
$password = New-Password -Length 20
```