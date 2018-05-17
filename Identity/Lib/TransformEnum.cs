using System;
using System.Linq;
using System.Collections.Generic;
using Identity.Models;
using System.Security.Claims;

namespace Identity.Lib
{
  public class TranformEnum<EnumRoles>
  {
    public TranformEnum()
    {
      if (!typeof(EnumRoles).IsEnum) 
      {
        throw new ArgumentException();
      }
    }
    public Dictionary<string, int> ToDictionary()
    {
      
      var values = Enum
        .GetValues(typeof(EnumRoles))
        .Cast<int>()
        .ToList();

      var names = Enum.GetNames(typeof(EnumRoles)).ToList();

      var dictionary = new Dictionary<string, int>();
      for (var i = 0; i < values.Count; i++)
      {
        dictionary.Add(names[i], values[i]);
      }

      return dictionary;
    }      
    public IList<Claim> ToClaimList()
    {
      var dictionary = ToDictionary();
      var names = Enum.GetNames(typeof(EnumRoles)).ToList();
      var claims = new List<Claim>();

      for (var i = 0; i < dictionary.Count; i++)
      {
        claims.Add(new Claim(ApplicationClaimTypes.Permission, dictionary[names[i]].ToString()));
      }
      return claims;
    }      
  }
}