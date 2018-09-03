using System;

namespace Nexus.Data 
{
    [Flags]
    public enum MembershipTypes: byte
    {
        None = 0,
        Guest = 1, // external user
        Member = 2, // basic
        Manager = 4, // can configure some things
        Owner = 8 // 
    }
}