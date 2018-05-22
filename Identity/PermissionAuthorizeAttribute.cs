using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Identity.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Identity.Lib;
using System.Collections.Generic;

namespace Identity
{
  public class PermissionAuthorizeAttribute : TypeFilterAttribute
  {
    public PermissionAuthorizeAttribute(params int[] perms) : base(typeof(PermissionAuthorizeFilter))
    {
      Arguments = new object[] { perms };
    }
    public PermissionAuthorizeAttribute(params string[] perms) : base(typeof(PermissionAuthorizeFilter))
    {
      Arguments = new object[] { perms.Select(v => Int32.Parse(v)) };
    }
  }
  public class PermissionAuthorizeFilter : IAuthorizationFilter
  {
    private readonly IList<int> _perms;
    public PermissionAuthorizeFilter(int[] perms)
    {
        _perms = perms;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var user = context.HttpContext.User;
    
      if (!user.Identity.IsAuthenticated) {
        context.Result = new UnauthorizedResult();
        return;
      }
      var claimsValues = user.Claims.Where(c => c.Type == ApplicationClaimTypes.Permission).Select(c => Convert.ToInt32(c.Value));
      foreach (var perm in _perms) {
        if (!claimsValues.Contains(perm)) {
          context.Result = new UnauthorizedResult();
          break;
        }
      }
    }
  }
}