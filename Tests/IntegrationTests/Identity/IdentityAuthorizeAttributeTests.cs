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

namespace Tests.IntegrationTests.Identity
{
    public class IdentityAuthorizeAttributeTests
    {
      private readonly IWebHostBuilder _hostBuider;
      private readonly TestServer _testServer;


      public IdentityAuthorizeAttributeTests()
      {
        _hostBuider = new WebHostBuilder()
          .ConfigureServices(services => {
            ConfigureIdentity
              .AddServices(services, dbOptions => {
                dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());
              });
            
            // services.AddRouting();
            services.AddMvc();
          })
          .Configure(app => {
            app.UseMvc(routes => {
              routes.MapRoute("default", "{controller=FakeIdentityAuthorizeTestsController}/{action=Index}/{id?}");
            });
            // var routerBuilder = new RouteBuilder(app);
            // RequestDelegate getIndex = c => {
            //   return c.Response.WriteAsync("test");
            // };
            // routerBuilder.MapGet("test", getIndex);

            // var routes = routerBuilder.Build();
            // app.UseRouter(routes);

          });
        
        _testServer = new TestServer(_hostBuider);
      }

      [Fact]
      public async Task TestInitializeWebHost()
      {
        var client = _testServer.CreateClient();
        var response1 = await client.GetAsync("/");
        var response2 = await client.GetAsync("/test");

        response1.EnsureSuccessStatusCode();
        response2.EnsureSuccessStatusCode();
        var result1 = await response1.Content.ReadAsStringAsync();
        var result2 = await response2.Content.ReadAsStringAsync();

        Assert.Equal("test", result1);
        Assert.Equal("test_route", result2);
      }
    }
}