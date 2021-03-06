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
using Identity.Lib;
using System.Text.RegularExpressions;

namespace Identity
{
  public class ConfigureIdentity
  {
    private readonly IConfiguration _config;
    private Action<IdentityOptions> _identityOptions;
    private Action<DbContextOptionsBuilder> _dbOptions;
    private Action<JwtBearerOptions> _jwtBearerOptions;

    public static ConfigureIdentity Make(IConfiguration config)
    {
      return new ConfigureIdentity(config); 
    }
    public static ConfigureIdentity Make()
    {
      return new ConfigureIdentity();
    }
    public ConfigureIdentity(IConfiguration config = null)
    {
      _config = config;
    }
    public ConfigureIdentity SetDbContextConfig(Action<DbContextOptionsBuilder> setupDbContext)
    {
      _dbOptions = setupDbContext;
      return this;
    }
    public ConfigureIdentity SetJwtBearerOptions(Action<JwtBearerOptions> jwtBearerOptions)
    {
      _jwtBearerOptions = jwtBearerOptions;
      return this;
    }
    public ConfigureIdentity SetIdentityOptions(Action<IdentityOptions> identityOptions)
    {
      _identityOptions = identityOptions;
      return this;
    }
    public void AddServices(IServiceCollection services)
    {
      if ((_config == null && _dbOptions == null) || (_config == null && _jwtBearerOptions == null)) {
        throw new ArgumentNullException("Please set config or sets for DbOptions and JwtBearerOptions");
      }
      // DbContextConfig
      var configureDbContext = _dbOptions ?? new Action<DbContextOptionsBuilder>((options) => {
        options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
      });
      // Identity config
      var configureIdentity = _identityOptions ?? new Action<IdentityOptions>((options) => {
        options.Lockout.AllowedForNewUsers = true;
      });
      //  jwtConfig
      var jwtBearerOptions = _jwtBearerOptions ?? new Action<JwtBearerOptions>((options) => {
        options.RequireHttpsMetadata = false;
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = _config["Jwt:Issuer"],
          ValidAudience = _config["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
        };
      });
      // Adicionando servicços
      services.AddDbContext<ApplicationIdentityDbContext>(configureDbContext);
      services.AddIdentity<ApplicationUser, ApplicationRole>(configureIdentity)
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()      
        .AddDefaultTokenProviders();
      // Auth
      services.AddAuthentication(options => {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(jwtBearerOptions);
    }
  }
}