using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Identity;
using Tests.IntegrationTests.Identity;
using Microsoft.AspNetCore.Identity;
using Identity.Models;
using System.ComponentModel.DataAnnotations;
using Identity.Lib;

namespace WebAPI.Controllers
{
    public class SignViewModel 
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    [Route("/")]
    public class FakeIdentityAuthorizeTestsController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Index()
        {
            return "test";
        }

        // GET api/values/5
        [HttpGet("test")]
        public string TestRoute()
        {
            return "test_route";
        }

        [IdentityAuthorize]
        // POST api/values
        [HttpGet("auth")]
        public string AuthorizedRoute([FromBody]string value)
        {
            return "authorized!";
        }

        // Test sigin in identity and return response with JWT Token
        [HttpPost("sigin")]
        public async Task<IActionResult> Post(
            [FromBody] SignViewModel signData, 
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] IdentityUserManager userManager
            )
        {
            var user = await userManager.FindByNameAsync(signData.Username);
            if (user == null) 
            {
                return BadRequest();
            }
            var result = await signInManager.PasswordSignInAsync(user, signData.Password, false, false);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            var claims  = await signInManager.CreateUserPrincipalAsync(user);            
            // var jwt = GenerateJWT(claims.Claims, )
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
