using System;
using Identity.Models;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Identity.Lib
{
  public class IdentityUserStore : UserStore<ApplicationUser, ApplicationRole, ApplicationIdentityDbContext, Guid>
  {
    private DbSet<ApplicationUser> UsersSet { get { return Context.Set<ApplicationUser>(); } }
    private DbSet<ApplicationRole> Roles { get { return Context.Set<ApplicationRole>(); } }
    private DbSet<IdentityUserClaim<Guid>> UserClaims { get { return Context.Set<IdentityUserClaim<Guid>>(); } }
    private DbSet<IdentityUserRole<Guid>> UserRoles { get { return Context.Set<IdentityUserRole<Guid>>(); } }
    public new ApplicationIdentityDbContext Context { get; private set; }

    public IdentityUserStore(ApplicationIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
    {
        Context = context;
    }
    public new async Task<IList<ApplicationRole>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var query = from userRole in UserRoles
                    join role in Roles on userRole.RoleId equals role.Id
                    where userRole.UserId.Equals(user.Id)
                    select role;
        
        return await query.ToListAsync(); 
    }
    public async Task AddToRoleAsync(ApplicationUser user, ApplicationRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(role.Name))
        {
            throw new ArgumentException("Please role give a name");
        }
        await UserRoles.AddAsync(CreateUserRole(user, role));
    }
  }
}