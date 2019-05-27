using System;
using System.Collections.Generic;

namespace NerdyMishka.EfCore.Identity
{
    public class User 
    {
        public User()
        {
            
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }


        /// <summary>
        /// Gets or sets the user's psuedonym
        /// </summary>
        /// <value></value>
        public string Pseudonym { get; set; }

        /// <summary>
        /// Gets or sets the user's primary email address.
        /// </summary>
        /// <value></value>
        public string Email { get; set; }

        public string DisplayName { get; set; }

        public bool IsActive { get; set; } = true;

        public int? PasswordPolicyId { get; set; }

        public virtual PasswordPolicy PasswordPolicy { get; set; } 

        public virtual PasswordLogin PasswordLogin { get; set; }

        public virtual ICollection<EmailAddress> EmailAddresses { get; set; }

        public virtual ICollection<Organization> Organization { get; set; }
    }
}
