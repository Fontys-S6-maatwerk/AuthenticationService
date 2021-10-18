using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Entity;
using AuthenticationService.Web.Entity;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Services
{
    /// <summary>
    /// The default microservice authentication gRPC client.
    /// </summary>
    public class AuthenticationService : AuthGRPCService.AuthGRPCServiceBase
    {
        /// <summary>
        /// Logger instance for the auth service.
        /// </summary>
        private readonly ILogger<AuthenticationService> logger;

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
        private readonly RoleManager<Role> roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="userManager">Injected identity manager.</param>
        /// <param name="roleManager">Injected identity role manager.</param>
        /// <param name="configuration">Injected configuration class.</param>
        /// <param name="logger">Injected logger.</param>
        public AuthenticationService(UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration configuration, ILogger<AuthenticationService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Tries to sign in a user based on the given username and password.
        /// </summary>
        /// <param name="request">Sign in request.</param>
        /// <param name="context">Server context.</param>
        /// <returns><see cref="SignInResponse"/> that contains the status of the request and
        /// if succesfull, returns a generated JWT token.
        /// </returns>
        public override async Task<AuthOperationResponse> SignIn(SignInRequest request, ServerCallContext context)
        {
            var userEmail = request.Email;
            var user = await this.userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);

            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Email not found."));
            }

            if (await this.userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: this.configuration["JWT:ValidIssuer"],
                    audience: this.configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                // return token.
                return new AuthOperationResponse
                {
                    Contents = new JwtSecurityTokenHandler().WriteToken(token),
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Password incorrect."));
            }

            throw new RpcException(new Status(StatusCode.Unauthenticated, "Failed to generate token."));
        }

        /// <summary>
        /// Tries to register an account in the Microsoft identity.
        /// </summary>
        /// <param name="request">Register request.</param>
        /// <param name="context">Server context.</param>
        /// <returns>The <see cref="ServerCallContext"/> with the status of the request.</returns>
        public override async Task<AuthOperationResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var user = await this.userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (user != null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Email already in use."));
            }

            // Create user object
            var newUser = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
               // Password = request.Password,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // TODO: add default user role to user.
            var result = await this.userManager.CreateAsync(newUser, request.Password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                // Something went wrong
                throw new RpcException(new Status(StatusCode.Aborted, "Failed to create account."));
            }

            // Account created
            return new AuthOperationResponse { Contents = result.Succeeded.ToString() };
        }
    }
}
