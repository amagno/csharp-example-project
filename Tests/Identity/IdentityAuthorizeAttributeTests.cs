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
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Tests.Identity
{
    public class IdentityAuthorizeAttributeTests : IDisposable
    {
      private readonly TestServer _testServer;
      public IdentityAuthorizeAttributeTests()
      {
        var tokenConfig = new TokenValidationParameters {
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateLifetime = false,
          ValidateIssuerSigningKey = true,
          ValidateActor = false,
          ValidateTokenReplay = false,
          ValidIssuer = JWTConfig.Audience,
          ValidAudience = JWTConfig.Audience,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTConfig.Key))
          
        };
        var builder = new WebHostBuilder()
          .ConfigureServices(services => {
            new ConfigureIdentity()
                .SetTokenParameters(tokenConfig)
                .SetDbContextConfig(dbOptions => {
                    dbOptions.UseInMemoryDatabase("TESTING_DATABASE_MEMORY");
                })
                .SetIdentityOptions(identityOptions => {
                    identityOptions.SignIn.RequireConfirmedEmail = false;
                    identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddServices(services);
            
           
            services.AddMvc(options => {
              // options.Filters.Add<IdentityAuthorizeAttribute>();
            });
          })
          .Configure(app => {
            app.UseAuthentication();
            app.UseMvc(routes => {
              routes.MapRoute("default", "{controller=FakeIdentityAuthorizeTestsController}/{action=Index}/{id?}");
            });
          });
          var testServer = new TestServer(builder);
          testServer.BaseAddress = new Uri("http://localhost");
          _testServer = testServer;
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
      public async Task TestUnauthorizedRoute()
      {
        var client = _testServer.CreateClient();
        var response = await client.GetAsync("/auth");

        var status = response.StatusCode;

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
      [Fact]
      public async Task TestLoginReturnJWTToken()
      {
        var client = _testServer.CreateClient();
        var userManager = _testServer.Host.Services.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
        var data = new Dictionary<string, string>();

        var user = new ApplicationUser {
          Email = "test@gmail.com",
          UserName = "testing"
        };

        data.Add("email", user.Email);
        data.Add("password", "Aa@123456");
        data.Add("username", user.UserName);
        var formData = new FormUrlEncodedContent(data);


        var createdUser = await userManager.CreateAsync(user, data["password"]);

        if (!createdUser.Succeeded) { throw new Exception("error on create user"); }
        // var responseRegister = await client.PostAsync("/register", formData);
        // responseRegister.EnsureSuccessStatusCode();

        var responseLogin = await client.PostAsync("/login", formData);
        responseLogin.EnsureSuccessStatusCode();

        var validator = new JwtSecurityTokenHandler();
        var resText = await responseLogin.Content.ReadAsStringAsync();
        var read = validator.CanReadToken(resText);
        Assert.True(read);
        
      }
      // Test usar authenticated acesse protected route
      [Fact]
      public async Task TestUserAuthorized()
      {
        // Create Services
        var client = _testServer.CreateClient();
        var userManager = _testServer.Host.Services.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
        var signInManager = _testServer.Host.Services.GetService(typeof(SignInManager<ApplicationUser>)) as SignInManager<ApplicationUser>;
        // create user
        var user = new ApplicationUser {
          Email = "test@gmail.com",
          UserName = "testing"
        };
        var password = "Aa@123456";
        var createdUser = await userManager.CreateAsync(user, password);
        if (!createdUser.Succeeded) { throw new Exception("error on create user"); }
        // create claims
        var principal = await signInManager.CreateUserPrincipalAsync(user);
        // generate token and validate then
        var token = GenerateJWT.Generate(principal.Claims, JWTConfig.Key, JWTConfig.Issuer, DateTime.Now.AddHours(1));
        var read = new JwtSecurityTokenHandler().CanReadToken(token);
        if (!read) { throw new Exception("token generated is invalid"); }
        // set token to request header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        // send request
        var response = await client.GetAsync("/auth");
        response.EnsureSuccessStatusCode();
        // get response of request
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("authorized!", result);
      }
      [Fact]
      public async Task TestAddPermissionsToUSerAndAuthorizeRouteWithPermissions()
      {
        var roleName = "testing_role";
        var client = _testServer.CreateClient();
        var userManager = _testServer.Host.Services.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
        var signInManager = _testServer.Host.Services.GetService(typeof(SignInManager<ApplicationUser>)) as SignInManager<ApplicationUser>;
        var roleManager = _testServer.Host.Services.GetService(typeof(RoleManager<ApplicationRole>)) as RoleManager<ApplicationRole>;

        await new InitializeIdentity(roleManager, userManager).RegisterPermissions(typeof(PermissionsTest)).SeedClaimsAsync(roleName);

        var user = new ApplicationUser {
          Email = "test@gmail.com",
          UserName = "testing"
        };
        var password = "Aa@123456";
        var createdUser = await userManager.CreateAsync(user, password);
        if (!createdUser.Succeeded) { throw new Exception("error on create user"); }

        var addToRole = await userManager.AddToRoleAsync(user, roleName);
        if (!addToRole.Succeeded) { throw new Exception("error on add user to role"); }

        var principal = await signInManager.CreateUserPrincipalAsync(user);
        var token = GenerateJWT.Generate(principal.Claims, JWTConfig.Key, JWTConfig.Issuer, DateTime.Now.AddHours(1));

        var read = new JwtSecurityTokenHandler().CanReadToken(token);
        if (!read) { throw new Exception("token generated is invalid"); }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

        var response = await client.GetAsync("/auth_roles");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        Assert.Equal("authorized!", result);
      }

  }
}