using System.Collections.Generic;
using Identity.Models;
using Identity.Lib;
using Xunit;
using Identity;
using System.Security.Claims;
using System;
using System.Linq;

namespace Tests.Identity
{
  public class TranformEnumRolesTest
  {
    [Fact]
    public void TranformToClaimsListTest()
    {

      var names = Enum.GetNames(typeof(EPermissionsTest)).ToList();
      var transformed = new TranformEnum<EPermissionsTest>().ToClaimList();

      for(var i = 0; i < names.Count; i++)
      {
        Assert.Equal((i + 1).ToString(), transformed[i].Value);
        Assert.Equal(ApplicationClaimTypes.Permission, transformed[i].Type);
      }
    }
  }
}