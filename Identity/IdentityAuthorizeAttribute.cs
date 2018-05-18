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
    public IdentityAuthorizeAttribute() : base(typeof(IdentityAuthorizeFilter))
    {
      // var cast = (int[])roles;
      // if (cast == null) 
      // {
      //   throw new Exception("Unable to cast roles");
      // }
      // var args = roles.ToList().Select(v => Convert.ToInt32(v));
      // var t = Convert.ToInt32(role.ToString());
      // Arguments = new object[] { t };
    }
  }
  public class IdentityAuthorizeFilter : IAuthorizationFilter
  {
    // private readonly IList<int> _roles;
    public IdentityAuthorizeFilter()
    {
        // _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var claims = context.HttpContext.User;
      
      if (!claims.Identity.IsAuthenticated) 
      {
        context.Result = new UnauthorizedResult();
        return;
      }

      // for (int i = 0; i < _roles.Count; i++)
      // {
      //   if (!claims.IsInRole(_roles[i]))
      //   {
      //     context.Result = new UnauthorizedResult();
      //     break;
      //   }
      // }
    }
  }
}