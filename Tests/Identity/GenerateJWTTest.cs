using System;
using System.Collections.Generic;
using System.Security.Claims;
using Identity.Lib;
using Xunit;
using Tests.Identity.Fixtures;

namespace Tests.Identity
{
    public class GenerateJWTTest
    {
      [Fact]
      public void TestGenerate()
      {
        var claims = new List<Claim> {
          new Claim("test", "1"),
          new Claim("test", "2"),
        };
        var expires = new DateTime().AddHours(1);
        var token = JWT.Generate(claims, JWTConfig.Key, "test", expires);

        Assert.True(!string.IsNullOrEmpty(token));
      }
    }
}