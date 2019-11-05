using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    public class PasswordGenerator
    {
        public static object RngCryptoServiceProvider { get; private set; }

        public static readonly string LatinAlphaUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static readonly string LatinAlphaLowerCase = "abcdefghijklmnopqrstuvwxyz";

        public static readonly string Digits = "0123456789";

        public static readonly string Hyphen = "-";
        public static readonly string Underscore = "-";

        public static readonly string Brackets = "[]{}()<>";

        public static readonly string Special = "~`&%$#@*+=|\\,:;^";

        public static readonly string Space = " ";

        private static readonly char[] SpecialChars = (Special + Hyphen + Underscore + Brackets + Space).ToCharArray();

        public static char[] CombineAll()
        {
            return Combine(
                LatinAlphaLowerCase,
                LatinAlphaUpperCase,
                Digits,
                Hyphen,
                Underscore,
                Brackets,
                Special,
                Space);
        }

        public static char[] Combine(params string[] lists)
        {
            if(lists != null && lists.Length > 0)
            {
                var sb = new StringBuilder();
                foreach(var list in lists)
                {
                    sb.Append(list);
                }

                return sb.ToString().ToCharArray();
            }

            return null;
        }

        public static string GenerateAsString(int length, char[] characters = null, Func<char[], bool> validate = null)
        {
            return new string(Generate(length, characters, validate));
        }

        public static char[] Generate(int length, char[] characters = null, Func<char[], bool> validate = null)
        {
            if (length < 1)
                throw new IndexOutOfRangeException($"length must be 1 or greater. {length}");

            if (characters == null)
                characters = Combine(LatinAlphaUpperCase, LatinAlphaLowerCase, Digits, "#@_-^+");

            if (validate == null)
                validate = Validate;


            var password = new char[length];
            var bytes = new byte[length];

            while (validate(password) == false)
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }

                for (var i = 0; i < length; i++)
                {
                    var randomIndex = bytes[i] % characters.Length;
                    password[i] = characters[randomIndex];
                }
            }

            return password;
        }

        public static SecureString GenerateAsSecureString(
            int length, 
            char[] characters = null, 
            Func<char[], bool> validate = null)
        {
            var password = Generate(length, characters, validate);
            var secureString = new SecureString();
            foreach (var c in password)
                secureString.AppendChar(c);

            Array.Clear(password, 0, password.Length);
            return secureString;
        }


        public static byte[] GenerateAsBytes(
            int length,
            char[] characters = null,
            Func<char[], bool> validate = null)
        {
            var password = Generate(length, characters, validate);
            var bytes = Encoding.UTF8.GetBytes(password);

            Array.Clear(password, 0, password.Length);
            return bytes;
        }


        private static bool Validate(char[] characters)
        {
            if (characters == null && characters.Length == 0)
                return false;

            bool lower = false,
                 upper = false,
                 digit = false,
                 special = false;

        

            for (var i = 0; i < characters.Length; i++)
            {
                var c = characters[i];
                if (char.IsDigit(c))
                    digit = true;

                if(Char.IsLetter(c))
                {
                    if (Char.IsUpper(c))
                        upper = true;
                    else if (Char.IsLower(c))
                        lower = true;
                }

                if (SpecialChars.Contains(c))
                    special = true;
             

                if (lower && upper && digit && special)
                    return true;
            }

            return false;
        }
    }
}
