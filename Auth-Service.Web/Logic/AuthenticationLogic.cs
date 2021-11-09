﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth_Service.Data.DTO;
using Auth_Service.Data.Entities;
using AuthenticationService.Web.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Auth_Service.Web
{
    /// <summary>
    /// The default microservice authentication client.
    /// </summary>
    public class AuthenticationLogic
    {
        /// <summary>
        /// Logger instance for the auth service.
        /// </summary>
        //private readonly ILogger<AuthenticationLogic> logger;

        /// <summary>
        /// Interface for reading the configuration file.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The dotnet user manager class.
        /// </summary>
        private readonly UserManager<User> userManager;

        /// <summary>
        /// The dotnet role manager class.
        /// </summary>
        //private readonly RoleManager<Role> roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationLogic"/> class.
        /// </summary>
        /// <param name="userManager">Injected identity manager.</param>
        /// <param name="roleManager">Injected identity role manager.</param>
        /// <param name="configuration">Injected configuration class.</param>
        /// <param name="logger">Injected logger.</param>
        public AuthenticationLogic(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;

        }

        /// <summary>
        /// Tries to sign in a user based on the given username and password.
        /// </summary>
        /// <param name="request">Sign in request.</param>
        /// <param name="context">Server context.</param>
        /// <returns><see cref="SignInResponse"/> that contains the status of the request and
        /// if succesfull, returns a generated JWT token.
        /// </returns>
        public async Task<Token> SignInAsync(SignInDTO userLogIn)
        {
            var userEmail = userLogIn.Email;
            var user2 = await this.userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);

            if (user2 == null)
            {
                throw new Exception("Email not found.");
            }

            if (await this.userManager.CheckPasswordAsync(user2, userLogIn.Password))
            {
                var token = CreateToken(user2);
                return token;
            }
            else
            {
                throw new Exception("Password incorrect.");
            }

            throw new Exception("Failed to generate token.");
        }

        public Token CreateToken(User user)
        {
            var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

            var para = this.configuration["JWT:Secret"];
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(para));
            var token = new JwtSecurityToken(
                issuer: this.configuration["JWT:ValidIssuer"],
                audience: this.configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new Token
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
            };
        }

    }
}
