using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using AttendanceWebApi.Models;
using AttendanceWebApi.Helpers;

namespace AttendanceWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private readonly attWebApiContext _context;
        private readonly IConfiguration _configuration;

        public TokenController(attWebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Create([FromBody] LoginDetails login)
        {
            var verifiedUser = this.UserValidated(login);
            if (verifiedUser != default(Users))
            {                
                var obj = new
                {
                    UserId = verifiedUser.Id,
                    verifiedUser.Username,
                    UserRoles = AppHelper.GetUserRoles(verifiedUser.Id, _context),
                    Token = GenerateToken(login.Username)
                };
                return new ObjectResult(obj);
            }            
            return new ObjectResult("InvalidCredentials");
        }
        
        /// <summary>
        /// Get valiadted user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private Users UserValidated(LoginDetails login) => _context.Users.SingleOrDefault(x => x.Username == login.Username && x.Password == login.Password);

        /// <summary>
        /// Generate token for specific user [http://www.blinkingcaret.com/2017/09/06/secure-web-api-in-asp-net-core/]
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Validation").GetSection("SecurityKey").Value));

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Email, ""),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            };

            var token = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Validation").GetSection("Issuer").Value,
                    audience: _configuration.GetSection("Validation").GetSection("Audience").Value,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddDays(10),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}