using Identity.Models;
using Tests.Identity.Fixtures;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Xunit;
using Identity;
using System.Linq;

namespace Tests.Identity
{
  public class PermissionsSeederTests
  {
    private readonly RoleManager<ApplicationRole> _roleManager;
    public PermissionsSeederTests()
    {
      var webHost = new WebHostFixture();
      _roleManager = webHost.GetRoleManager();
    }
    [Fact]
    public async Task TestSeedPermissions()
    {

      var seeder = new PermissionsSeeder(_roleManager);

      await seeder.RegisterPermissions(typeof(PermissionsTest)).SeedAsync();

      var roles = _roleManager.Roles.ToList();
      var claims = await _roleManager.GetClaimsAsync(roles[0]);

      Assert.True(roles.Count == 1);
      Assert.True(claims.Count == 2);
      Assert.Equal("1", claims[0].Value);
      Assert.Equal("2", claims[1].Value);
    
    }
  }
}