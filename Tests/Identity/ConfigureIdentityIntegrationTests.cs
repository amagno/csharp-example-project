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

namespace Tests.IntegrationTests.Identity
{
  public class WebHostFixture : IDisposable
  {
    private readonly IWebHost _webHost;
    private IdentityUserManager _userManager;
    private RoleManager<ApplicationRole> _roleManager;
    public WebHostFixture()
    {
      _webHost = WebHost.CreateDefaultBuilder()
          .ConfigureServices(services => {
            ConfigureIdentity
              .AddServices(services, dbOptions => {
                dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());
              });

              var provider = services.BuildServiceProvider();
              _roleManager = provider.GetService<RoleManager<ApplicationRole>>();
              _userManager = provider.GetService<IdentityUserManager>();
          })
          .UseStartup<FakeStartup>()
          .Build();

    }
    public IdentityUserManager GetUserManager()
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

            await initializer.RegisterEnumRoles<ERolesTest>().Seed();
        
            var roles = roleManager.Roles.ToList();
            var names = Enum.GetNames(typeof(ERolesTest)).ToList();
            var myRole = roleManager.FindByNameAsync(names[0]);
            Assert.True(roles.Count == 2);
            Assert.True(myRole != null);

            for (var i = 0; i < names.Count; i++)
            {
                Assert.Equal(names[i], roles[i].Name);
            }
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

            Assert.True(created.Succeeded);
        }
      }
      [Fact]
      public async Task TestCreateUserRoles()
      {
        using (var fixture = new WebHostFixture())
        {
            var userManager = fixture.GetUserManager();
            var roleManager = fixture.GetRoleManager();

            var initializer = new InitializeIdentity(roleManager);

            await initializer.RegisterEnumRoles<ERolesTest>().Seed();
        
            var roles = roleManager.Roles.ToList();
            var names = Enum.GetNames(typeof(ERolesTest)).ToList();


            var user = new ApplicationUser {
              Email = "test@gmail.com",
              UserName = "testing"
            };
            // IN SEQUENCE
            var created = await userManager.CreateAsync(user, "Aa@123456");
            // 
            var result = await userManager.AddToRolesAsync(user, roles); 
            //  
            var userRoles = await userManager.GetRolesAsync(user);

            Assert.True(result.Succeeded);
            Assert.True(roles.Count == 2);
            Assert.True(userRoles.Count == 2);
        }
      }
    }
}