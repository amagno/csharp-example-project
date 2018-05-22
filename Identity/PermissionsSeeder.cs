using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Lib;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;


namespace Identity 
{
  public class PermissionsSeeder
  {
    private readonly RoleManager<ApplicationRole> _roleManager;
    private IList<Claim> _claims;

    public PermissionsSeeder(RoleManager<ApplicationRole> roleManager)
    {
      _roleManager = roleManager ?? throw new ArgumentNullException("roleManager");
    }

    public PermissionsSeeder RegisterPermissions(Type type)
    {
      if (!type.IsClass) {
        throw new ArgumentException("type");
      }
      _claims = new TranformPermissions(type).ToClaimList();
      return this;
    }

    public async Task SeedAsync(string defaultRoleName = "default")
    {
      if (_roleManager == null) {
        throw new ArgumentNullException();
      }
      if (_claims.Count == 0) {
        throw new Exception("Please register enum roles whit: 'RegisterEnumRoles<Enum>()'");
      }
      var defaultRole = await CreateRoleAsyncIfNotExists(defaultRoleName);
      var existsClaims = await _roleManager.GetClaimsAsync(defaultRole);
      foreach(var claim in _claims) 
      {
        var any = existsClaims.Any(c => c.Value == claim.Value);
        if (!any) {
          var createdClaim = await _roleManager.AddClaimAsync(defaultRole, claim);
          if (!createdClaim.Succeeded) {
            throw new Exception(createdClaim.Errors.ToString());
          }
        }
      }
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
  }
}