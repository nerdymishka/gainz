using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// The number of rounds that Salsa can perform
    /// </summary>
    public enum Salsa20Rounds
    {
        Eight = 8,
        Ten = 10,
        Twelve = 12,
        Twenty = 20
    }
}
