using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace LabSwwashbuckle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string secrectKey;

        public UserController(IConfiguration config)
        {
            secrectKey = config.GetSection("settings").GetSection("secretKey").ToString();
        }

        [HttpPost]
        [Route("Validate")]
        public IActionResult Validate([FromBody] User usuario)
        {
            if(usuario.Mail == "test@test.com" && usuario.Password == "Pa$$w0rd1")
            {
                var keyBytes = Encoding.ASCII.GetBytes(secrectKey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Mail));

                var tokenDescription = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescription);

                string tokenCreate = tokenHandler.WriteToken(tokenConfig);

                return StatusCode(StatusCodes.Status200OK, new { token = "Bearer " + tokenCreate });   
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

        }
    }
}
