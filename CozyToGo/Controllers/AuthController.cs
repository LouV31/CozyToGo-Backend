using CozyToGo.Data;
using CozyToGo.DTO.UserDTO;
using CozyToGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CozyToGo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Email already exist");
            }
            var newUser = new User
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                City = user.City,
                Address = user.Address,
                ZipCode = user.ZipCode,
                Phone = user.Phone,
                Role = "User"
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return Ok(newUser);

        }

        [HttpPost("authentication")]
        public async Task<IActionResult> LogIn(LoginDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid User");
            }
            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password)) // Qui mancava una parentesi
            {
                return BadRequest("Invalid Password");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name, user.Email.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString())
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var authenticatedUser = new
            {
                Id = user.IdUser,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                City = user.City,
                Address = user.Address,
                Token = tokenHandler.WriteToken(token)
            };

            // Convert authenticatedUser to a dictionary so we can add properties conditionally
            var result = new RouteValueDictionary(authenticatedUser);

            if (!string.IsNullOrEmpty(user.Address2))
            {
                result.Add("Address2", user.Address2);
            }

            if (!string.IsNullOrEmpty(user.Address3))
            {
                result.Add("Address3", user.Address3);
            }

            return Ok(result);
        }

    }
}
