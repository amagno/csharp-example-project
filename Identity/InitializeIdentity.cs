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
    public static PermissionsSeeder SeedPermissions<TPermissions>(RoleManager<ApplicationRole> roleManager)
    {
      return new PermissionsSeeder(roleManager).RegisterPermissions(typeof(TPermissions));
    }

  }
}