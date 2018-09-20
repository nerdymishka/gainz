using System;

namespace Nexus.Data
{
    [Flags]
    public enum ResourceActions: short
    {
        None = 0,
        List = 1,
        Read = 2,
        Write = 4,
        Delete = 8,
        Execute = 16,
        Audit = 32,
        Billing = 64,

        Reader = List | Read,
        Contributor = List | Read | Write, 
        Operator = List | Read | Execute,
        Auditor = List | Read | Audit,
        Accounting = List | Read | Billing,
        Manager = List | Read | Write | Delete | Execute,
        Onwer = List | Read | Write | Delete | Execute | Audit | Billing
    }    
}