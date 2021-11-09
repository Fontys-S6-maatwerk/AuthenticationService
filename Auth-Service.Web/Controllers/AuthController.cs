using Auth_Service.Data;
using Auth_Service.Data.Entities;
using Auth_Service.Data.DTO;
using AuthenticationService.Web.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Auth_Service.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthenticationLogic authLogic;
        public UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public AuthController(ApplicationDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpGet]
        public string test()
        {
            return "Wrm werkt nooit iets gelijk in 1 keer?!";
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO user)
        {
            try
            {
                var newToken = LogInAsync(user);
                return StatusCode(200, newToken);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] RegisterDTO userdto)
        {
            try
            {
                User newUser = new User
                {
                    UserName = userdto.Email,
                    Email = userdto.Email,
                    FirstName = userdto.FirstName,
                    LastName = userdto.LastName,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                //DO NOT MOVE LINE 73 OTHERWISE IT BREAKS!!!
                var result = await userManager.CreateAsync(newUser, userdto.Password);

                Token token = CreateToken(newUser);
                return StatusCode(201, token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        // TODO: implement patch service
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] User value)
        {
        }
        // TODO: implement delete sequence using rabitmq.
        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
        }

        private async Task<Token> LogInAsync(SignInDTO user)
        {
            var userEmail = user.Email;
            var user2 = await this.userManager.FindByEmailAsync(userEmail).ConfigureAwait(false);

            if (user == null)
            {
                throw new Exception("Email not found.");
            }

            if (await this.userManager.CheckPasswordAsync(user2, user.Password))
            {
                var token = CreateToken(user2);
                return token;
            }
            else
            {
                throw new Exception("Password incorrect.");
            }
            
        }

        private Token CreateToken (User user)
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
