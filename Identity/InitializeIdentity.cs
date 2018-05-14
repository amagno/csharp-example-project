using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Identity.Lib;
using System.Linq;
using System.Threading.Tasks;

namespace Identity
{
  public class InitializeIdentity
  {

      private IList<ApplicationRole> _roles;
      private readonly UserManager<ApplicationUser> _userManager; 
      private readonly RoleManager<ApplicationRole> _roleManager;

    public InitializeIdentity(
      UserManager<ApplicationUser> userManager, 
      RoleManager<ApplicationRole> roleManager
      )
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }
    public InitializeIdentity RegisterEnumRoles<EnumRoles>()
    {
      _roles = new TranformEnumRoles<EnumRoles>().Transform();
      return this;
    }
    public async Task Seed()
    {
      if (_roles.Count == 0)
      {
        throw new Exception("Please register enum roles whit: 'RegisterEnumRoles<Enum>()'");
      }
      foreach(var role in _roles) 
      {
        var exists = await _roleManager.RoleExistsAsync(role.Name);
        if (!exists)
        {
         var result = await _roleManager.CreateAsync(role);

         if (!result.Succeeded)
         {
           throw new Exception(result.Errors.ToString());
         }
        }
      }
    }
  }
}