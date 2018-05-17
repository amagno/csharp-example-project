using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Identity.Lib;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Identity
{
  public class InitializeIdentity
  {

      private IList<Claim> _claims;
      private readonly UserManager<ApplicationUser> _userManager; 
      private readonly RoleManager<ApplicationRole> _roleManager;

    public InitializeIdentity(
      RoleManager<ApplicationRole> roleManager,
      UserManager<ApplicationUser> userManager = null      
      )
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }
    public InitializeIdentity RegisterEnumClaims<TEnum>()
    {
      _claims = new TranformEnum<TEnum>().ToClaimList();
      return this;
    }
    public async Task<ApplicationRole> CreateRoleAsyncIfNotExists(string roleName)
    {
      var exists = await _roleManager.RoleExistsAsync(roleName);
      if (exists) {
        return await _roleManager.FindByNameAsync(roleName);
      }
      var role = new ApplicationRole { Name = roleName };
      var createdRole = await _roleManager.CreateAsync(role);

      if (!createdRole.Succeeded) {
        throw new Exception(createdRole.Errors.ToString());
      }
      return role;
    }
    public async Task SeedClaimsAsync(string defaultRoleName = "default")
    {
      if (_claims.Count == 0) {
        throw new Exception("Please register enum roles whit: 'RegisterEnumRoles<Enum>()'");
      }
      var defaultRole = await CreateRoleAsyncIfNotExists(defaultRoleName);
      foreach(var claim in _claims) 
      {
        var createdClaim = await _roleManager.AddClaimAsync(defaultRole, claim);
        if (!createdClaim.Succeeded) {
          throw new Exception(createdClaim.Errors.ToString());
        }
      }
    }
  }
}