using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Identity.Lib
{
  public class IdentityUserManager : UserManager<ApplicationUser>
  {

    public IdentityUserManager(
      IUserStore<ApplicationUser> store, 
      IOptions<IdentityOptions> optionsAccessor, 
      IPasswordHasher<ApplicationUser> passwordHasher, 
      IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
      IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
      ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
      IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger
      ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }
    private IdentityUserStore GetUserStore()
    {
        var cast = Store as IdentityUserStore;
        if (cast == null)
        {
            throw new NotSupportedException("Resources.StoreNotIUserRoleStore");
        }
        return cast;
    }
    public new async Task<IList<ApplicationRole>> GetRolesAsync(ApplicationUser user)
    {
        ThrowIfDisposed();
        var userStore = GetUserStore();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        return await userStore.GetRolesAsync(user, CancellationToken);
    }
    public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<ApplicationRole> roles)
    {
        ThrowIfDisposed();
        var userStore = GetUserStore();

        foreach(var role in roles.Distinct())
        {

            if (await userStore.IsInRoleAsync(user, role.Name))
            {
                return IdentityResult.Failed(ErrorDescriber.UserAlreadyInRole(role.Name));
            }
            await userStore.AddToRoleAsync(user, role);
        }

        return await UpdateAsync(user);
    }
  }
}