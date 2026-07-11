using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Enhanzer.Backend.Models;
using Enhanzer.Backend.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Enhanzer.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, AppDbContext context, IConfiguration config)
        {
            _authService = authService;
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { Message = "Email and Password are required." });

            var externalResponse = await _authService.AuthenticateExternallyAsync(request.Email, request.Password);

            // Validate response and extract User_Locations
            if (externalResponse == null || externalResponse.Status_Code != 200 || externalResponse.Response_Body == null || !externalResponse.Response_Body.Any())
            {
                return Unauthorized(new { Message = "Invalid credentials or external authentication failed." });
            }

            var userLocations = externalResponse.Response_Body.First().User_Locations;
            if (userLocations == null)
            {
                return Unauthorized(new { Message = "No locations found for this user." });
            }

            // Save or Update locations in DB
            foreach (var loc in userLocations)
            {
                var existingLoc = await _context.Location_Details.FindAsync(loc.Location_Code);
                if (existingLoc == null)
                {
                    _context.Location_Details.Add(new LocationDetail
                    {
                        Location_Code = loc.Location_Code,
                        Location_Name = loc.Location_Name
                    });
                }
                else
                {
                    existingLoc.Location_Name = loc.Location_Name;
                }
            }

            await _context.SaveChangesAsync();

            // Generate JWT Token
            var tokenString = GenerateJSONWebToken(request.Email);

            return Ok(new { Token = tokenString, Locations = userLocations });
        }

        private string GenerateJSONWebToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
