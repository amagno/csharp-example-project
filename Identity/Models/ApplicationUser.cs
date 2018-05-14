using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Username { get; }
       
        
        
    }
}