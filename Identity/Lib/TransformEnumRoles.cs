using System;
using System.Linq;
using System.Collections.Generic;
using Identity.Models;

namespace Identity.Lib
{
  public class TranformEnumRoles<EnumRoles>
  {
    public TranformEnumRoles()
    {
      if (!typeof(EnumRoles).IsEnum) 
      {
        throw new ArgumentException();
      }
    }

    public IList<ApplicationRole> Transform()
    {
      var values = Enum
        .GetValues(typeof(EnumRoles))
        .Cast<int>()
        .ToList();

      var names = Enum.GetNames(typeof(EnumRoles)).ToList();
      var roles = new List<ApplicationRole>();

      for(var i = 0; i < values.Count; i++)
      {
        var role = new ApplicationRole {
          Name = names[i],
          Number = values[i]
        };
        roles.Add(role);
      }

      return roles;
    }      
  }
}