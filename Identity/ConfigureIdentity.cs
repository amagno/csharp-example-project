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

namespace Identity
{
  public class ConfigureIdentity
  {
    private readonly IConfiguration _config;
    private Action<IdentityOptions> _identityOptions;
    private Action<DbContextOptionsBuilder> _dbOptions;
    private TokenValidationParameters _tokenParameters;

    public ConfigureIdentity(IConfiguration config = null)
    {
      _config = config;
    }
    public ConfigureIdentity SetDbContextConfig(Action<DbContextOptionsBuilder> setupDbContext)
    {
      _dbOptions = setupDbContext;
      return this;
    }
    public ConfigureIdentity SetTokenParameters(TokenValidationParameters tokenParameters)
    {
      _tokenParameters = tokenParameters;
      return this;
    }
    public ConfigureIdentity SetIdentityOptions(Action<IdentityOptions> identityOptions)
    {
      _identityOptions = identityOptions;
      return this;
    }
    public void AddServices(IServiceCollection services)
    {
      if ((_config == null && _dbOptions == null) || (_config == null && _tokenParameters == null)) 
      {
        throw new ArgumentNullException();
      }

      var configureDbContext = _dbOptions ?? new Action<DbContextOptionsBuilder>((options) => {
        options.UseSqlServer(_config.GetConnectionString("DefaultConnection"));
      });
      var configureIdentity = _identityOptions ?? new Action<IdentityOptions>((options) => {
        options.Lockout.AllowedForNewUsers = true;
      });

      var configureJwtToken = _tokenParameters ?? new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _config["Issuer"],
        ValidAudience = _config["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
      };
      // Adicionando servicços
      services.AddDbContext<ApplicationIdentityDbContext>(configureDbContext);
      services.AddIdentity<ApplicationUser, ApplicationRole>(configureIdentity)
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        // .AddUserStore<IdentityUserStore>()        
        // .AddUserManager<IdentityUserManager>()
        // .AddClaimsPrincipalFactory<IdentityClaimsPrincipalFactory>()        
        .AddDefaultTokenProviders();
        
      services.AddAuthentication(options => {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(options => {
        // options.Audience = configureJwtToken.ValidAudience ?? throw new Exception("Invalid audience");
        // options.Authority = configureJwtToken.ValidIssuer ?? throw new Exception("Invalid issuer");
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = configureJwtToken;
        options.IncludeErrorDetails = true;
      });
    }
  }
}