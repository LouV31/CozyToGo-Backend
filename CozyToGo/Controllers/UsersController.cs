using CozyToGo.Data;
using CozyToGo.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CozyToGo.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("edit")]
        [Authorize]
        public async Task<IActionResult> EditUser(EditUserDto userDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == userId);
            if (user == null)
            {
                return BadRequest("Invalid User");
            }
            user.Name = userDto.Name;
            user.Surname = userDto.Surname;
            user.Email = userDto.Email;
            user.Address = userDto.Address;
            user.Address2 = userDto.Address2;
            user.Address3 = userDto.Address3;
            user.City = userDto.City;
            user.ZipCode = userDto.ZipCode;
            user.Phone = userDto.Phone;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
