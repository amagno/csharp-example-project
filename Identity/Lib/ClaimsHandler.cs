using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Identity.Models;

namespace Identity.Lib
{
    public class ClaimsHandler
    {
        private IEnumerable<Claim> _claims { get; } = new List<Claim>();

        public ICollection<Claim> Claims => _claims.ToArray();
        

        
    }
}