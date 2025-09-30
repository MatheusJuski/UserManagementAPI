using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementAPI.Models;
using UserManagementAPI.Services;
using UserManagementAPI.DTOs;



namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Usa o método do UserService que valida hash
        var user = await _userService.ValidateUserCredentialsAsync(request.Username, request.Password);
        if (user == null)
            return Unauthorized("Usuário ou senha inválidos");

        

        var token = GenerateJwtToken(user);
        
        

        // Gera e salva refresh token
            await _userService.SetRefreshTokenAsync(user);

        return Ok(new
        {
            token,
            refreshToken = user.RefreshToken, // envia refresh token
            expires = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")
            )
        });
    }
        [HttpPost("refresh-token")]
        [AllowAnonymous]
    
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            var user = await _userService.GetUserByRefreshTokenAsync(request.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return Unauthorized("Token inválido ou expirado");

            var newJwtToken = GenerateJwtToken(user);
            await _userService.SetRefreshTokenAsync(user); // renova refresh token

            return Ok(new
            {
                token = newJwtToken,
                refreshToken = user.RefreshToken
            });
        }



        // 🔑 Função privada para gerar token
        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User") // Admin/User
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO de login
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
