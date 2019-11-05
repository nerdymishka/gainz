using System;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Identity
{

    public class ApiKey
    {
        public ApiKey()
        {
            this.ClientId = Guid.NewGuid().ToString();
        }

        public int Id { get; set; }

        public string ClientId { get; set; } 

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
      
        public string Value  { get; set; }

        public DateTime? ExpiresAt { get; set; }

        

     
    }
}