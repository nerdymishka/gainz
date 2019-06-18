using System;
using System.Collections.Generic;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Identity
{

    public class ApiKeyRole
    {
        public int ApiKeyId { get; set; }

        public virtual ApiKey ApiKey { get; set; }


        public virtual int RoleId { get; set; }

        public virtual Role Role { get; set; }        
    }
}