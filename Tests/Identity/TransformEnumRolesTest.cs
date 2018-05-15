using System.Collections.Generic;
using Identity.Models;
using Identity.Lib;
using Xunit;

namespace Tests.UnitTests.Identity
{
  enum RolesFixture
  {
      TestRole1 = 1,
      TestRole2 = 2
  }
  public class TranformEnumRolesTest
  {
    [Fact]
    public void TranformTest()
    {
      var roles = new List<ApplicationRole> {
        new ApplicationRole {
          Name = "TestRole1",
          Number = 1
        },
        new ApplicationRole {
          Name = "TestRole2",
          Number = 2
        },
      };

      var transformed = new TranformEnumRoles<RolesFixture>().Transform();

      for(var i = 0; i < roles.Count; i++)
      {
        Assert.Equal(roles[i].Name, transformed[i].Name);
        Assert.Equal(roles[i].Number, transformed[i].Number);
      }
    }
  }
}