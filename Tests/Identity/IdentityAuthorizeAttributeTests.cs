using Microsoft.AspNetCore.Hosting;
using Xunit;
using Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Identity;
using Identity.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Controllers;
using System.Net.Http;
using System.Collections.Generic;
using Identity.Lib;

namespace Tests.Identity
{
    public class IdentityAuthorizeAttributeTests : IDisposable
    {
      private readonly TestServer _testServer;
      public IdentityAuthorizeAttributeTests()
      {
        var tokenConfig = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = "localhost",
          ValidAudience = "localhost",
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTConfig.Key))
        };
        var builder = new WebHostBuilder()
          .ConfigureServices(services => {
            new ConfigureIdentity()
                .SetTokenParameters(tokenConfig)
                .SetDbContextConfig(dbOptions => {
                    dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());
                })
                .SetIdentityOptions(identityOptions => {
                    identityOptions.Lockout.AllowedForNewUsers = true;
                    identityOptions.SignIn.RequireConfirmedEmail = false;
                    identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddServices(services);

            services.AddMvc(options => {
              options.Filters.Add<IdentityAuthorizeAttribute>();
            });
          })
          .Configure(app => {
            app.UseMvc(routes => {
              routes.MapRoute("default", "{controller=FakeIdentityAuthorizeTestsController}/{action=Index}/{id?}");
            });
            app.UseAuthentication();
          });
        
        _testServer = new TestServer(builder);
      }
      public void Dispose()
      {
        _testServer.Dispose();
      }

      [Fact]
      public async Task TestInitializeWebHost()
      {
        var client = _testServer.CreateClient();
        var response1 = await client.GetAsync("/");

        response1.EnsureSuccessStatusCode();
        var result1 = await response1.Content.ReadAsStringAsync();

        Assert.Equal("test", result1);
      }
      [Fact]
      public async Task TestAuthorizedRouteGetErrorWebHost()
      {
        var client = _testServer.CreateClient();
        var response = await client.GetAsync("/auth");

        var status = response.StatusCode;

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
      }
      [Fact]
      public async Task TestRegisterAndLogin()
      {
        var client = _testServer.CreateClient();
        var data = new Dictionary<string, string>();

        data.Add("email", "test@gmail.com");
        data.Add("password", "Aa@123456");
        data.Add("username", "testing");

        var formData = new FormUrlEncodedContent(data);

        var responseRegister = await client.PostAsync("/register", formData);
        
        responseRegister.EnsureSuccessStatusCode();

        var created = await responseRegister.Content.ReadAsStringAsync();

        Console.WriteLine("Create at route => ");
        Console.WriteLine(created);

        var responseLogin = await client.PostAsync("/login", formData);
        
        responseLogin.EnsureSuccessStatusCode();

        var validator = new JwtSecurityTokenHandler();
        var resText = await responseLogin.Content.ReadAsStringAsync();
        var read = validator.CanReadToken(resText);
        Assert.True(read);
        
      }
      [Fact]
      public async Task TestRegister()
      {
        var client = _testServer.CreateClient();
        // var userManager = _testServer.Host.Services.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

        var data = new Dictionary<string, string>();
        data.Add("username", "testing");
        data.Add("email", "test@gmail.com");
        data.Add("password", "Aa@123456");
        var formData = new FormUrlEncodedContent(data);


        var response = await client.PostAsync("/register", formData);
        response.EnsureSuccessStatusCode();

        var status = response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();
        // var users = userManager.Users.ToList();
        Console.WriteLine("GENERATED GUID => ");
        Console.WriteLine(content);
        
        Guid parsed = Guid.Parse(content);
        Assert.Equal(HttpStatusCode.Created, status);
        
      }

  
  }
}