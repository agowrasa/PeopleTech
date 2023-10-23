using AngularAuthAPI.DBContext.AngularAuthAPI.DBContext;
using AngularAuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AngularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUser login)
        {
            var user = _dbContext.LoginUsers.SingleOrDefault(u => u.Email == login.Email);

            if (user == null || !CheckPassword(user.Password, login.Password))
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));

            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = true, // Change as needed
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            return Ok(new { token });
        }

        private string GenerateJwtToken(LoginUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("G6HJyxRWTq0riAA4S5QyaNLYF+OU52Lb3vgCP50N6FIohSUvQ/dW8dLf8j+NkNKlh6sIlHKsqPtb0YoCPLNPPA==")); // Replace with a secure secret key
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "User") // You can set roles here if needed
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5205",
                audience: "http://localhost:4200",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), // Token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool CheckPassword(string hashedPassword, string plainPassword)
        {
            // Implement password hashing and comparison logic here.
            // Use a secure hashing library like BCrypt for password hashing.
            // For example:
            // return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

            // For demonstration, we're using a simple comparison.
            return hashedPassword == plainPassword;
        }
    }
}
