using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Identity.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Identity
{
  public class IdentityAuthorizeAttribute : TypeFilterAttribute
  {
    public IdentityAuthorizeAttribute(params Enum[] roles) : base(typeof(IdentityAuthorizeFilter))
    {
      
    }
  }
  public class IdentityAuthorizeFilter : IAuthorizationFilter
  {
    private readonly ApplicationRole[] _roles;
    public IdentityAuthorizeFilter(ApplicationRole[] roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      context.Result = new UnauthorizedResult();
    }
  }
}