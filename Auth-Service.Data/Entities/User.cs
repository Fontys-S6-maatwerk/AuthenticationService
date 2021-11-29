using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth_Service.Data.Entities
{
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the foreign key of the record inside the person table that is associated with this user.
        /// </summary>
        public int? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the last name of the account owner.
        /// </summary>
        public string Password { get; set; }
    }
}

