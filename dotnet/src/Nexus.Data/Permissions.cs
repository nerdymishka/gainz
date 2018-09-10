using System;

namespace Nexus.Data
{
    [Flags]
    public enum Permissions: short 
    {
        None = 0,

        /// <summary>
        /// The List permission allows a subject to see the resource listed and 
        /// know of its existence. 
        /// </summary>
        List = 1,

        /// <summary>
        /// The Read permission allows a subject to read values from a resource.
        /// </summary>
        Read = 2,

        /// <summary>
        /// The Write permission allows a subject to write values to a resource.
        /// </summary>
        Write = 4,

        /// <summary>
        /// The Delete permission allows a subject to delete a resource.
        /// </summary>
        Delete = 8,
        
        ///<summary>
        /// The Execution permission allows a subject to invoke the 
        /// default run action.
        /// </summary>
        Execute = 16,

        /// <summary>
        /// Special permissions allows users to invoke custom actions
        /// that are not the default Run action.
        /// </summary>
        Special = 32,

        /// <summary>
        /// The Audit permission allows a subject to read audit information.
        /// </summary>
        Audit = 64,

        /// <summary>
        /// The Billing permission allows a ubject to read billing information
        /// about a resource, if applicable.
        /// </summary>
        Billing = 128,

        /// <summary>
        /// The Reader permission is the default for a reader role, which includes
        /// List and Read perssions.
        /// </summary>
        Reader = List | Read,

        /// <summary>
        /// The Contributor permission is the default for a contributor role, which includes
        /// List, Read, and Write perssions.
        /// </summary>
        Contributor = List | Read | Write,

        /// <summary>
        /// The Operator permission is the default for a operator role, which includes
        /// List, Read, Execute, and Special permissions.
        /// </summary>
        Operator = List | Read | Execute | Special,

        /// <summary>
        /// The Manager permission is the default for a manager role, which includes
        /// List, Read, Write, and Delete permissions.
        /// </summary>
        Manager = List | Read | Write | Delete,

        /// <summary>
        /// The Auditor permission is the default for a auditor role, which includes
        /// List,Read, and Audit permissions.
        /// </summary>
        Auditor = List | Read | Audit,

        /// <summary>
        /// The BillingInfo permission is the default for a  role, which includes
        /// List and Billing permissions.
        /// </summary>
        BillingInfo = List | Billing,

        /// <summary>
        /// The Owner permission is the default for a admin/owner role, which includes
        /// List, Read, Write, Delete, Execute, Special, Audit, and Billing
        /// </summary>
        Owner = List | Read | Write | Delete | Execute | Special | Audit | Billing
    }    
}