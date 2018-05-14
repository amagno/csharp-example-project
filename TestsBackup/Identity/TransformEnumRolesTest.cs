using Xunit;

namespace Tests.Identity
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
      var roles = new {
        new ApplicationRole()
      };
    }
  }
}