using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UniversityApp.Core.Entites;
using UniversityApp.Service.Dtos.UserDtos;
using UniversityApp.Service.Exceptions;
using UniversityApp.Service.Interfaces;

namespace UniversityApp.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public string Login(UserLoginDto loginDto)
        {
            AppUser user = _userManager.FindByNameAsync(loginDto.UserName).Result;

            if (user == null || !_userManager.CheckPasswordAsync(user,loginDto.Password).Result) throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim("FullName", user.FullName));

            string audience = "https://localhost:7061/";
            string issuer = "https://localhost:7061/";
            string secret = "my super secret security jwt token my super secret security jwt token";
            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                issuer: issuer,
                audience: audience,
                signingCredentials: creds,
                expires: DateTime.Now.AddDays(3)
                );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token); 

            return tokenStr;
        }
    }
}
