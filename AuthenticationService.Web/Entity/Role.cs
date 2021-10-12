using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Web.Entity
{
    public class Role : IdentityRole
    {
        /// <summary>
        /// Gets or sets the description of the current role object.
        /// </summary>
        public string Description { get; set; }
    }
}
