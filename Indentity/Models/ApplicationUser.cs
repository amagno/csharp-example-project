using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Indentity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; }
        public string Username { get; }
       
        
        public IList<Claim> GetClaimsFromUser(ApplicationUser user)
        {
            return new[]
            {
                new Claim("id", new Guid().ToString()), 
                new Claim("username", user.Username), 
                new Claim("email", user.Email)
            };
        }
    }
}