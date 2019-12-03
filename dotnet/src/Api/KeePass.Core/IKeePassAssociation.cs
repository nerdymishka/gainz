using System;

namespace NerdyMishka.KeePass
{
    public interface IKeePassAssociation
    {
        string Window { get; set; }

        string KeystrokeSequence { get; set; }
    }
}