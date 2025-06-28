using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using crud_api.Common;
using crud_api.Context;
using crud_api.DTOS;
using crud_api.Entities;
using crud_api.Settings;

namespace crud_api.Services;

public class AuthService(AppDbContext context, IOptions<JwtSettings> jwtSettings)
{
    private readonly AppDbContext _context = context;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    private string GenerateJWT(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// public async Task<AuthServiceResult> Login(UserLoginDto userLoginDto)
// {
//     var dbUser = await _context.Users
//         .Where(u => u.Email == userLoginDto.Email)
//         .FirstOrDefaultAsync();
//
//     if (dbUser is null)
//     {
//         return new AuthServiceResult
//         {
//             Status = ServiceResultStatus.NotFound,
//             Message = "User not found"
//         };
//     }
//
//     if (dbUser.PasswordHash != userLoginDto.Password)
//     {
//         return new AuthServiceResult
//         {
//             Status = ServiceResultStatus.InvalidInput,
//             Message = "Password is incorrect"
//         };
//     }
// }
//
// }