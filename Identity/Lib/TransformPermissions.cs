using System;
using System.Linq;
using System.Collections.Generic;
using Identity.Models;
using System.Security.Claims;
using System.Reflection;

namespace Identity.Lib
{
  public class TranformPermissions
  {
    private readonly Type _type;
    public TranformPermissions(System.Type type)
    {
      if (!type.IsClass) {
        throw new ArgumentException();
      }
      _type = type;
    }
    public Dictionary<string, string> ToDictionary()
    {
      var fields = GetConstants(_type);

      var dictionary = new Dictionary<string, string>();
      foreach (var prop in fields) {
        var name = prop.Name;
        var value = prop.GetValue(null).ToString();

        if (value == null || name == null) {
          throw new ArgumentNullException();
        }

        dictionary.Add(name, value);
      }
      return dictionary;
    }      
    public IList<Claim> ToClaimList()
    {
      var dictionary = ToDictionary();
      var claims = new List<Claim>();

      foreach (KeyValuePair<string, string> entry in dictionary) {
        claims.Add(new Claim(entry.Key, entry.Value.ToString()));
      }
      return claims;
    }
    private List<FieldInfo> GetConstants(Type type)
    {
      FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
          BindingFlags.Static | BindingFlags.FlattenHierarchy);

      return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
    }     
  }
}