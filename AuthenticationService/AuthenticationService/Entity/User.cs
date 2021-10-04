namespace AuthenticationService.Entity
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// User class.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the foreign key of the record inside the person table that is associated with this user.
        /// </summary>
        public int? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the account owner.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account owner.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account owner.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account owner.
        /// </summary>
        public string Password { get; set; }
    }
}
