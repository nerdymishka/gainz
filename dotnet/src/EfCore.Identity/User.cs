using System;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Identity
{
    public class User 
    {
        public User()
        {
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }


        /// <summary>
        /// Gets or sets the user's psuedonym
        /// </summary>
        /// <value></value>
        public string Pseudonym { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user's primary email address.
        /// </summary>
        /// <value></value>
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        public bool IsPhoneConfirmed { get; set; } = false;



        public bool IsActive { get; set; } = true;

        public int? MultiFactorPolicyId { get; set; }

        public int? PasswordPolicyId { get; set; }

        public virtual int? OrganizationId { get; set; }


    }
}
