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
  public static class PermissionsTestFixture
  {
    public const int Permission1 = 1;
    public const int Permission2 = 2;
  }
  public class TranformPermissionsTest
  {
    [Fact]
    public void TranformToClaimsListTest()
    {
      var transformed = new TranformPermissions(typeof(PermissionsTestFixture)).ToClaimList();
      Assert.Equal(PermissionsTestFixture.Permission1.ToString(), transformed[0].Value);
      Assert.Equal(PermissionsTestFixture.Permission2.ToString(), transformed[1].Value);
      Assert.Equal(ApplicationClaimTypes.Permission, transformed[0].Type);
      Assert.Equal(ApplicationClaimTypes.Permission, transformed[1].Type);
    }
  }
}