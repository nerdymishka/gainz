using System;


namespace NerdyMishka.EfCore.Identity
{
    /// <summary>
    /// Password login information for a given user.  The class stores
    /// the password and lockout information. 
    /// </summary>
    public class PasswordLogin
    {
        /// <summary>
        /// Id of the user.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="PasswordLogin" /> must map to an existing user or not
        ///         exist. 
        ///     </para>
        /// </remarks>
        /// <value></value>
        public int UserId { get; set; }

        public User User  { get; set; }

        /// <summary>
        /// Gets or sets the encoded password hash. 
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The password must be stored as a one way hash. Additional information
        ///         can be stored if the data is base 64 encoded.  The password column length
        ///         should be 1024.
        ///     </para>
        /// </remarks>
        /// <value></value>
        public string Password  { get; set; }

        public DateTime? PasswordUpdatedAt { get; set; }

        /// <summary>
        /// Gets ors sets when the password expires. If the value is null
        /// the password does not expire per NIST SP 800 63b. 
        /// </summary>
        /// <remarks>
        ///     <para>
        ///       [NIST SP 800 63b](https://pages.nist.gov/800-63-3/sp800-63b.html) 
        ///       states the following: "Verifiers SHOULD NOT require memorized secrets
        ///       to be changed arbitrarily (e.g., periodically). However, verifiers
        ///       SHALL force a change if there is evidence of compromise of the authenticator."
        ///     </para>
        ///     <para>
        ///       Thus passwords should not be rotated unless there is a compelling reason
        ///       to other than "best practices" as most users will generally change only
        ///       one character in an existing password, making it easy to brute force
        ///       the password, making rotation fairly meaningless.
        ///     </para>
        /// </remarks>
        /// <value></value>
        public DateTime? PasswordExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the number of login failures.
        /// </summary>
        /// <value></value>

        public short? FailureCount { get; set; }

        /// <summary>
        /// Gets or sets when the latest login failures started to 
        /// occur.  If the value is null, there has not been a recent failure.
        /// </summary>
        /// <value></value>

        public DateTime? FailureStartedAt { get; set;}

        /// <summary>
        /// Gets or sets if the account is currently locked out.
        /// </summary>
        /// <value></value>
        public bool IsLockedOut { get; set; }

        /// <summary>
        /// Gets or sets when the lockout started at.
        /// </summary>
        /// <value></value>
        public DateTime? LockOutStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the last login date time.
        /// </summary>
        /// <value></value>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Gets or set the last ip address.
        /// </summary>
        /// <value></value>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets when this record was last updated.
        /// </summary>
        /// <value></value>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Is the password locked from too many attempts?
        /// </summary>
        /// <param name="isAuthenicated">Was the password authenticated?</param>
        /// <param name="span">The timespan for the lockout period.</param>
        /// <param name="count">The number of attempts.</param>
        /// <returns>True, if the password login is locked; otherwise, false.</returns>

        public bool IsLocked(bool isAuthenicated, TimeSpan span, short numberOfAttemptsAllowed = 5)
        {
            if(isAuthenicated)
            {
                if(this.IsLockedOut)
                {
                    if(!this.LockOutStartedAt.HasValue)
                        this.LockOutStartedAt = DateTime.UtcNow;

                    // if locked out, you autheniciate, and your past the
                    // expiration time... unlock the account 
                    var diff = this.LockOutStartedAt.Value - DateTime.UtcNow;
                    if(diff > span)
                    {
                        this.FailureStartedAt = null;
                        this.FailureCount = 0;
                        this.Unlock();
                        this.LastLoginAt = DateTime.UtcNow;
                        return false;
                    }

                    // if you're not past the expiration time, you're penalized for the attempt
                    this.LockOutStartedAt = DateTime.UtcNow;
                    return true;
                }

                this.FailureCount = null;
                this.FailureStartedAt = null;
                this.LastLoginAt = DateTime.UtcNow;
                this.UpdatedAt = DateTime.UtcNow;
                return false;
            }


            if(this.IsLockedOut)
            {
                if(!this.LockOutStartedAt.HasValue)
                    this.LockOutStartedAt = DateTime.UtcNow;

                // the attempt must reset the lockout period.
                var dif = this.LockOutStartedAt.Value - DateTime.UtcNow;
                if(dif <= span)
                {
                    this.LockOutStartedAt = DateTime.UtcNow;
                    this.FailureCount++;
                    return true;
                }
                else 
                {
                    // lockout expired, lets reset the failure clock
                    // and lets limit the number of attempts to more
                    // more after this.
                    this.LockOutStartedAt = null;
                    this.FailureStartedAt = DateTime.UtcNow;
                    this.FailureCount = (short)(numberOfAttemptsAllowed - 2); 
                }
            }

            
            if(!this.FailureStartedAt.HasValue)
                this.FailureStartedAt = DateTime.UtcNow;

            if(!this.FailureCount.HasValue)
                this.FailureCount = 0;
            
            this.FailureCount++;

            if(this.FailureCount >= numberOfAttemptsAllowed)
            {
                // welp you're now locked out.
                this.Lock();
                return true;
            }
            

            return false;
        }

        public void Lock()
        {
            this.LockOutStartedAt = DateTime.UtcNow;
            this.IsLockedOut = true;
            this.UpdatedAt = this.LockOutStartedAt;
        }

        public void Unlock()
        {
            this.LockOutStartedAt = null;
            this.IsLockedOut = false;
            this.FailureCount = null;
            this.FailureStartedAt = null;
            this.UpdatedAt = DateTime.UtcNow;
        }
    }
}