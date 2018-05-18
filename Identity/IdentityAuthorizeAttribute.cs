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
  public class IdentityAuthorizeAttribute : TypeFilterAttribute
  {
    public IdentityAuthorizeAttribute(params int[] perms) : base(typeof(IdentityAuthorizeFilter))
    {
      // var cast = (int[])roles;
      // if (cast == null) 
      // {
      //   throw new Exception("Unable to cast roles");
      // }
      // var args = roles.ToList().Select(v => Convert.ToInt32(v));
      // var t = Convert.ToInt32(role.ToString());
      Arguments = new object[] { perms };
    }
  }
  public class IdentityAuthorizeFilter : IAuthorizationFilter
  {
    private readonly IList<int> _perms;
    public IdentityAuthorizeFilter(int[] perms)
    {
        _perms = perms;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var user = context.HttpContext.User;
    
      if (!user.Identity.IsAuthenticated) 
      {
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