using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Identity.Lib
{
  class IdentityClaimsPrincipal : ClaimsPrincipal
  {
    public IdentityClaimsPrincipal()
    {
    }

    public IdentityClaimsPrincipal(IEnumerable<ClaimsIdentity> identities) : base(identities)
    {
    }

    public IdentityClaimsPrincipal(BinaryReader reader) : base(reader)
    {
    }

    public IdentityClaimsPrincipal(IIdentity identity) : base(identity)
    {
    }

    public IdentityClaimsPrincipal(IPrincipal principal) : base(principal)
    {
    }

    protected IdentityClaimsPrincipal(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public virtual bool IsInRole(int role)
    {
        return false;
    }
  }
}
