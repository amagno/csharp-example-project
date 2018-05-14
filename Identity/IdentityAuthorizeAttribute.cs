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
    public IdentityAuthorizeAttribute(params int[] roles) : base(typeof(IdentityAuthorizeFilter))
    {
      Arguments = new object[] { roles };
    }
  }
  public class IdentityAuthorizeFilter : IAuthorizationFilter
  {
    private readonly IList<int> _roles;
    public IdentityAuthorizeFilter(int[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var claims = context.HttpContext.User as IdentityClaimsPrincipal;

      for (int i = 0; i < _roles.Count; i++)
      {
        if (!claims.IsInRole(_roles[i]))
        {
          context.Result = new UnauthorizedResult();
          break; 
        }
      }
    }
  }
}