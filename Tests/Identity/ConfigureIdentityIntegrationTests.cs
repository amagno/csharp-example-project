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
using Tests.Identity.Fixtures;

namespace Tests.Identity
{
  public class ConfigureIdentityIntegrationTests
  {
      [Fact]
      public async Task TestInitializeIdentitySeedRoles()
      {
        using (var fixture = new WebHostFixture())
        {
            var roleManager = fixture.GetRoleManager();
            await InitializeIdentity.SeedPermissions<PermissionsTest>(roleManager).SeedAsync();
        
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
            await InitializeIdentity.SeedPermissions<PermissionsTest>(roleManager).SeedAsync();

            var defaultRole = roleManager.Roles.ToList()[0];
            var claims = await roleManager.GetClaimsAsync(defaultRole);

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
      [Fact]
      public async Task TestCreateUserAndDuplicatesClaims()
      {
        using (var fixture = new WebHostFixture())
        {
            var userManager = fixture.GetUserManager();
            var roleManager = fixture.GetRoleManager();
            await InitializeIdentity.SeedPermissions<PermissionsTest>(roleManager).SeedAsync();
            await InitializeIdentity.SeedPermissions<PermissionsTest>(roleManager).SeedAsync();

            var defaultRole = roleManager.Roles.ToList()[0];
            var claims = await roleManager.GetClaimsAsync(defaultRole);

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
      [Fact]
      public async Task TestInitializeWithConfig()
      {
        using (var fixture = new WebHostFixtureWithConfig())
        {
            var userManager = fixture.GetUserManager();
            var roleManager = fixture.GetRoleManager();
            await InitializeIdentity.SeedPermissions<PermissionsTest>(roleManager).SeedAsync();

            var defaultRole = roleManager.Roles.ToList()[0];
            var claims = await roleManager.GetClaimsAsync(defaultRole);

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