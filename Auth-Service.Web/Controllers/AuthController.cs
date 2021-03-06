using Auth_Service.Data;
using Auth_Service.Data.Entities;
using Auth_Service.Data.DTO;
using AuthenticationService.Web.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Auth_Service.Web.Logic;

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
        public EventbusSend eventbusSend;
        private readonly IConfiguration configuration;

        public AuthController(ApplicationDbContext context, UserManager<User> userManager, IConfiguration Configuration)
        {
            _context = context;
            this.userManager = userManager;
            configuration = Configuration;
            authLogic = new AuthenticationLogic(userManager, configuration);
            eventbusSend = new EventbusSend();
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
                Token newToken = await authLogic.SignInAsync(user);

                if(newToken == null)
                {
                    return StatusCode(400, new { errorMessage = "invalid username/password" });
                }

                return StatusCode(200, newToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = "something went wrong", internalError = ex.Message });
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> PostAsync([FromBody] RegisterDTO userdto)
        {
            try
            {
                User newUser = new User
                {
                    UserName = userdto.Email,
                    Email = userdto.Email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                //DO NOT MOVE LINE BELOW OTHERWISE IT BREAKS!!!
                var result = await userManager.CreateAsync(newUser, userdto.Password);

                if (result == null)
                {
                    return StatusCode(400, new { errorMessage = "invalid username/password" });
                }

                EventBusSendUserDTO sendUser = new EventBusSendUserDTO
                {
                    FirstName = userdto.FirstName,
                    LastName = userdto.LastName,
                    Email = userdto.Email
                };

                //Call eventbus
                eventbusSend.SendUser(sendUser);

                Token token = authLogic.CreateToken(newUser);

                return StatusCode(200, token);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { errorMessage = "something went wrong", internalError = ex.Message });
            }
            
        }
    }
}
