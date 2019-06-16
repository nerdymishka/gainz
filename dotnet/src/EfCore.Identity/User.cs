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

        /// <summary>
        /// Gets or sets the user's primary email address.
        /// </summary>
        /// <value></value>
        [Hash]
        public byte[] EmailHash { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        public bool IsPhoneConfirmed { get; set; } = false;

        public string DisplayName { get; set; }

        public bool IsActive { get; set; } = true;

        public int? MultiFactorPolicyId { get; set; }

        public virtual MultiFactorPolicy MultiFactorPolicy { get; set; }

        public int? PasswordPolicyId { get; set; }

        public virtual PasswordPolicy PasswordPolicy { get; set; } 

        public virtual PasswordLogin PasswordLogin { get; set; }

        public virtual ICollection<Phone> Phones { get; set; }

        public virtual ICollection<EmailAddress> EmailAddresses { get; set; }

        public virtual int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<ApiKey> ApiKeys { get; set; }


        public virtual IEnumerable<Role> Roles 
            => this.UserRoles?.Select(o => o.Role) ?? Enumerable.Empty<Role>();
    }
}
