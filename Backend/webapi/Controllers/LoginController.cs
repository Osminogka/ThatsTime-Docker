using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using webapi.Models;
using webapi.Models.User;
using webapi.DL.Repositories;

namespace webapi.Controllers
{
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    [ApiController]
    [Route("api/auth")]
    public class ApiAuthController : MyBaseController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IBaseRepository<UserInfo> _service;
        private readonly IConfiguration Configuration;

        public ApiAuthController(IUsersRepository usersRepository,IConfiguration config, IBaseRepository<UserInfo> service)
        {
            _usersRepository = usersRepository;
            Configuration = config;
            _service = service;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> ApiSignIn([FromBody] webapi.Models.User.SigningCredentials creds)
        {
            LoginResponse response = new LoginResponse();
            response.Message = "Invalid username or password";
            Microsoft.AspNetCore.Identity.SignInResult result = new Microsoft.AspNetCore.Identity.SignInResult();
            IdentityUser user = new IdentityUser();

            try{
                user = await _usersRepository.GetByUsername(creds.Username);
                if (user == null)
                    return Ok(response);
                result = await _usersRepository.CheckPasswordSignInAsync(user, creds.Password);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
            if (result.Succeeded)
            {
                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    }),
                    Expires = DateTime.MaxValue,
                    SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        Configuration["Jwt:Key"])),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityToken secToken = new JwtSecurityTokenHandler().CreateToken(descriptor);
                response.Success = true;
                response.Message = "Successfully signed in";
                response.Token = handler.WriteToken(secToken);
            }
                
            return Ok(response);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> ApiSignUp([FromBody] SingupCredentials creds)
        {
            IdentityUser user = new IdentityUser();
            IdentityResult result = new IdentityResult();

            try
            {
                user = new IdentityUser { UserName = creds.Username, Email = creds.Email };
                result = await _usersRepository.Create(user, creds.Password);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
            var response = new LoginResponse();

            if (result.Succeeded)
            {
                UserInfo newUser = new UserInfo()
                {
                    UserName = user.UserName
                };

                try
                {
                    await _service.AddAsync(newUser);
                    await _service.SaveChanges();
                }
                catch(Exception ex)
                {
                    return HandleException(ex);
                }

                SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    }),
                    Expires = DateTime.MaxValue,
                    SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        Configuration["Jwt:Key"])),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                SecurityToken secToken = new JwtSecurityTokenHandler().CreateToken(descriptor);
                response.Success = true;
                response.Message = "Account is created";
                response.Token = handler.WriteToken(secToken);
            }
            else
                response.Message = "Invalid username";
            return Ok(response);
        }
    }
}
