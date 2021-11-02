using AuthenticationService.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Web.Controller
{
    public class AuthenticationController
    {
        private AuthenticationService authService = new AuthenticationService();

        [HttpPost]
        public string SignIn(User user)
        {
            string token = authService.SignIn(user);
            return token;
        }

        [HttpPost]
        public string Register(User user)
        {
            var newUser = new User
            {
                Email = user.Email,
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            string result = authService.Register(newUser);
            return result;
        }
    }
}
