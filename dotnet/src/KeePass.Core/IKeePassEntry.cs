using System;
using System.Collections.Generic;
using System.Security;

namespace NerdyMishka.KeePass
{
    public interface IKeePassEntry : IKeePassNode
    {
        IKeyPassEntryFields Fields { get; }

        string ForegroundColor { get; set; }

        string BackgroundColor { get; set; }

        string OverrideUrl { get; set; }

        string UserName { get; set; }

        string Url { get; set; }

        IList<string> Tags { get; }

        IList<ProtectedString> Strings { get; }

        IKeePassAutoType AutoType { get; set; }

        IList<IKeePassEntry> History { get; }

        bool IsHistorical { get; set; }

        IList<ProtectedBinary> Binaries { get; }

        bool PreventAutoCreate { get; set; }

        byte[] CustomIconUuid { get; set; }

        IKeePassEntry CopyTo(IKeePassEntry destination, bool cleanHistory = false);

        string UnprotectPassword();

        byte[] UnprotectPasswordAsBytes();

        SecureString UnprotectPasswordAsSecureString();

        void SetPassword(byte[] password);

        void SetPassword(SecureString password);

        void SetPassword(string password);
    }
}