using System;


namespace NerdyMishka.Windows
{

    
    [CLSCompliant(false)]
    public enum Persistence: uint
    {
        Session = 1,
        LocalMachine = 2,
        Enterprise = 3
    }

    [CLSCompliant(false)]
    public enum CredentialFlags : uint
    {
        None = 0x0,
        PromptNow = 0x2,
        UsernameTarget = 0x4
    }

    [CLSCompliant(false)]
    public enum CredentialsType : uint
    {
        Generic = 1,
        DomainPassword = 2,
        DomainCertificate = 3,
        DomainVisiblePassword = 4,
        GenericCertificate = 5,
        DomainExtended = 6,
        Maximum = 7,
        MaximumEx = (Maximum + 1000),
    }
}
