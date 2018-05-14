using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Identity.Models;
using System;
using Microsoft.AspNetCore.Identity;

namespace Identity.Data
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationIdentityDbContext(DbContextOptions options) : base(options)
        {

        
        }
    }
}