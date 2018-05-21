using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Lib
{
    public class JWT
    {
        private readonly string _key;
        private readonly string _issuer;
        public JWT(string key, string issuer)
        {
            _key = key;
            _issuer = issuer;
        }
        public string Generate(IEnumerable<Claim> claims)
        {
            return JWT.Generate(claims, _key, _issuer, DateTime.Now.AddMinutes(30));
        }
        public string Generate(IEnumerable<Claim> claims, DateTime expires)
        {
            return JWT.Generate(claims, _key, _issuer, expires);
        }
        public static string Generate(IEnumerable<Claim> claims, string key, string issuer, DateTime? expires)
        {
            var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(symmKey, SecurityAlgorithms.HmacSha256);

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