namespace NerdyMishka.EfCore.Identity
{
    public enum PasswordComposition : byte 
    {
        UppercaseLetter = 1,
        LowercaseLetter = 2,
        Digit = 3,
        SpecialCharacter = 4
    }
}