using Microsoft.AspNetCore.Hosting;
using Xunit;
using Xunit.Sdk;
using Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Identity;
using Identity.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore;
using Identity.Lib;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.IdentityModel.Tokens;

namespace Tests.Identity
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

  public class ConfigureIdentityIntegrationTests
  {
      [Fact]
      public async Task TestInitializeIdentitySeedRoles()
      {
        using (var fixture = new WebHostFixture())
        {
            var roleManager = fixture.GetRoleManager();

            var initializer = new InitializeIdentity(roleManager);

            await initializer.RegisterPermissions(typeof(PermissionsTest)).SeedClaimsAsync();
        
            var roles = roleManager.Roles.ToList();
            var claims = await roleManager.GetClaimsAsync(roles[0]);

            Assert.True(roles.Count == 1);
            Assert.True(claims.Count == 2);
            Assert.Equal("1", claims[0].Value);
            Assert.Equal("2", claims[1].Value);
        }
      }
      [Fact]
      public async Task TestCreateUser()
      {
        using (var fixture = new WebHostFixture())
        {
            var userManager = fixture.GetUserManager();

            var user = new ApplicationUser {
              Email = "test@gmail.com",
              UserName = "testing"
            };

            var created = await userManager.CreateAsync(user, "Aa@123456");
            var find = await userManager.FindByEmailAsync(user.Email);
            var users = userManager.Users.ToList();
            Assert.True(created.Succeeded);
            Assert.Equal(find.Email, user.Email);
            Assert.Single(users);

        }
      }
      [Fact]
      public async Task TestCreateUserClaims()
      {
        using (var fixture = new WebHostFixture())
        {
            var userManager = fixture.GetUserManager();
            var roleManager = fixture.GetRoleManager();

            var initializer = new InitializeIdentity(roleManager);

            await initializer.RegisterPermissions(typeof(PermissionsTest)).SeedClaimsAsync();
            var defaultRole = roleManager.Roles.ToList()[0];
            var claims = await roleManager.GetClaimsAsync(defaultRole);
            // var names = Enum.GetNames(typeof(ERolesTest)).ToList();


            var user = new ApplicationUser {
              Email = "test@gmail.com",
              UserName = "testing"
            };
            // IN SEQUENCE
            var created = await userManager.CreateAsync(user, "Aa@123456");
            // 
            var result = await userManager.AddClaimsAsync(user, claims);
            //  
            var userClaims = await userManager.GetClaimsAsync(user);

            Assert.True(result.Succeeded);
            Assert.Equal(2, claims.Count());
            Assert.Equal(2, userClaims.Count());
        }
      }
    }
}