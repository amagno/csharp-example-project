using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Identity.Models;
using Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity
{
  public class ConfigureIdentity
  {
    private static readonly IConfiguration _config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
    public static void AddServices(
      IServiceCollection services,
      Action<DbContextOptionsBuilder> setupDbContext = null,      
      Action<IdentityOptions> setupIdentity = null
      )
    {
      var configureDbContext = setupDbContext ?? new Action<DbContextOptionsBuilder>((options) => {
        options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
      });
      var configureIdentity = setupIdentity ?? new Action<IdentityOptions>((options) => {
        options.Lockout.AllowedForNewUsers = true;
      });
      services.AddDbContext<ApplicationIdentityDbContext>(configureDbContext);
      services.AddIdentity<ApplicationUser, ApplicationRole>(configureIdentity)
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        .AddDefaultTokenProviders();
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters 
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = _config["Issuer"],
          ValidAudience = _config["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
        };
      });     
    }
  }
}