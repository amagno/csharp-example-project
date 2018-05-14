using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Lib
{
  class IdentityClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
  {
    public new IdentityUserManager UserManager { get; } 
    public IdentityClaimsPrincipalFactory(
        IdentityUserManager userManager, 
        RoleManager<ApplicationRole> roleManager, 
        IOptions<IdentityOptions> options
        ) : base(userManager, roleManager, options)
    {
        UserManager = userManager;
    }
    public virtual new async Task<IdentityClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        var id = await GenerateClaimsAsync(user);
        
        return new IdentityClaimsPrincipal(id);
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var id = await base.GenerateClaimsAsync(user);
        if (UserManager.SupportsUserRole)
        {
            var roles = await UserManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                // id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, role.Name));
                id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, role.Number.ToString()));
                id.AddClaim(new Claim(role.Name, role.Number.ToString()));
                if (RoleManager.SupportsRoleClaims)
                {
                    var res = await RoleManager.FindByNameAsync(role.Name);
                    if (res != null)
                    {
                        id.AddClaims(await RoleManager.GetClaimsAsync(res));
                    }
                }
            }
        }
        return id;
    }
  }

}
