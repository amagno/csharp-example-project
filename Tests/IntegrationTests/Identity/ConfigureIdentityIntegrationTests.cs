using Microsoft.AspNetCore.Hosting;
using Xunit;
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

namespace Tests.IntegrationTests.Identity
{

    public class ConfigureIdentityIntegrationTests
    {
      
      [Fact]
      public async Task TestInitializeIdentitySeedRoles()
      {
        var builder = WebHost.CreateDefaultBuilder()
          .ConfigureServices(services => {
            ConfigureIdentity
              .AddServices(services, dbOptions => {
                dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());
              });
            services.AddScoped<IdentityUserManager>();
          })
          .UseStartup<FakeStartup>()
          .Build();

        var userManager = (IdentityUserManager)builder
          .Services
          .GetService(typeof(IdentityUserManager));

        var roleManager = (RoleManager<ApplicationRole>)builder
          .Services
          .GetService(typeof(RoleManager<ApplicationRole>));

        var initializer = new InitializeIdentity(userManager, roleManager);

        await initializer.RegisterEnumRoles<ERolesTest>().Seed();
        
        var roles = roleManager.Roles.ToList();
        var names = Enum.GetNames(typeof(ERolesTest)).ToList();

        Assert.True(roles.Count == 2);

        for (var i = 0; i < names.Count; i++)
        {
          Assert.Equal(names[i], roles[i].Name);
        }
      }
    }
}