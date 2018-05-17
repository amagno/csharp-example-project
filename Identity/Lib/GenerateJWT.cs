using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Lib
{
    public class GenerateJWT
    {
        public static string Generate(IEnumerable<Claim> claims, string key, string issuer, DateTime? expires)
        {
            var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(symmKey, SecurityAlgorithms.EcdsaSha256);

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: expires ?? DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}