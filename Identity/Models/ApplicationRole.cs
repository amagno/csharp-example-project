using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{

    public class ApplicationRole : IdentityRole<Guid>
    {
      public int Number { get; set; }      
        
    }
}