using System;
using Identity;
using Identity.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IO;

namespace Tests.Identity.Fixtures
{
  public class WebHostFixture : IDisposable
  {
    private readonly IWebHost _webHost;
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<ApplicationRole> _roleManager;
    public WebHostFixture()
    {
        _webHost = WebHost.CreateDefaultBuilder()
            .ConfigureServices(services => {
                new ConfigureIdentity()
                    .SetDbContextConfig(dbOptions => {
                        dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    })
                    .SetJwtBearerOptions(jwtBearerOptions => {})
                    .AddServices(services);
                var provider = services.BuildServiceProvider();
                _roleManager = provider.GetService<RoleManager<ApplicationRole>>();
                _userManager = provider.GetService<UserManager<ApplicationUser>>();
            })
            .UseStartup<FakeStartup>()
            .Build();

    }
    public UserManager<ApplicationUser> GetUserManager()
    {
      return _userManager ?? throw new Exception("User manager is null");
    }
    public RoleManager<ApplicationRole> GetRoleManager()
    {
      return _roleManager ?? throw new Exception("Role manager is null");
    }
    public void Dispose()
    {
        System.Console.Out.WriteLine("Disposing fixture");        
        _webHost.Dispose();
    }
  }
  public class WebHostFixtureWithConfig : IDisposable
  {
    private readonly IWebHost _webHost;
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<ApplicationRole> _roleManager;
    public WebHostFixtureWithConfig()
    {
        _webHost = WebHost.CreateDefaultBuilder()
            .ConfigureServices(services => {
                var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory() + "..\\..\\..\\..\\..\\Identity")
                        .AddJsonFile("appsettings.json")
                        .Build();
              
                ConfigureIdentity.Make(config)
                .SetDbContextConfig(options => {
                  options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                })
                .AddServices(services);
                var provider = services.BuildServiceProvider();
                _roleManager = provider.GetService<RoleManager<ApplicationRole>>();
                _userManager = provider.GetService<UserManager<ApplicationUser>>();
            })
            .UseStartup<FakeStartup>()
            .Build();

    }
    public UserManager<ApplicationUser> GetUserManager()
    {
      return _userManager ?? throw new Exception("User manager is null");
    }
    public RoleManager<ApplicationRole> GetRoleManager()
    {
      return _roleManager ?? throw new Exception("Role manager is null");
    }
    public void Dispose()
    {
        System.Console.Out.WriteLine("Disposing fixture");        
        _webHost.Dispose();
    }
  }
  public class WebHostTestServer : IDisposable
  {
    private readonly TestServer _testServer;
    public WebHostTestServer()
    {
      var tokenConfig = new TokenValidationParameters {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = false,
          ValidateIssuerSigningKey = true,
          ValidateActor = false,
          ValidateTokenReplay = false,
          ValidIssuer = JWTConfig.Audience,
          ValidAudience = JWTConfig.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTConfig.Key))
          
      };
      var builder = new WebHostBuilder()
        .ConfigureServices(services => {
          ConfigureIdentity.Make()
              .SetDbContextConfig(dbOptions => {
                  dbOptions.UseInMemoryDatabase("TESTING_DATABASE_MEMORY");
              })
              .SetIdentityOptions(identityOptions => {
                  identityOptions.SignIn.RequireConfirmedEmail = false;
                  identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
              })
              .SetJwtBearerOptions(jwtBearerOptions => {
                jwtBearerOptions.RequireHttpsMetadata = false;
                jwtBearerOptions.IncludeErrorDetails = true;
                jwtBearerOptions.TokenValidationParameters = tokenConfig;
              })
              .AddServices(services);
      
            services.AddMvc(options => {
              // options.Filters.Add<IdentityAuthorizeAttribute>();
            });
          })
          .Configure(app => {
            app.UseAuthentication();
            app.UseMvc(routes => {
              routes.MapRoute("default", "{controller=FakeIdentityAuthorizeTestsController}/{action=Index}/{id?}");
            });
          });
          var testServer = new TestServer(builder);
          testServer.BaseAddress = new Uri("http://localhost");
          _testServer = testServer;
    }
    public TestServer GetServer()
    {
      return _testServer;
    }
    public void Dispose()
    {
      _testServer.Dispose();
    }
  }
}