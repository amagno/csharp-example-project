using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Identity;
using Microsoft.AspNetCore.Identity;
using Identity.Models;
using System.ComponentModel.DataAnnotations;
using Identity.Lib;
using Tests.Identity;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{   
    public class RegisterModel {
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    [Route("/")]
    public class FakeIdentityAuthorizeTestsController : Controller
    {
        // GET api/values
        private void ThrowIfRegisterModelInvalid(RegisterModel signInData)
        {
            if (string.IsNullOrEmpty(signInData.username) || string.IsNullOrEmpty(signInData.email) || string.IsNullOrEmpty(signInData.password))
            {
                throw new Exception("parameters invalid");
            }
        }
        [HttpGet]
        public string Index()
        {
            return "test";
        }

        // Test authorized route
        [IdentityAuthorize]
        [HttpGet("auth")]
        public string AuthorizedRoute()
        {
            return "authorized!";
        }
        [IdentityAuthorize(PermissionsTest.Permission1, PermissionsTest.Permission2)]
        [HttpGet("auth_roles")]
        public string AuthorizedRoutePermission()
        {
            return "authorized!";
        }

        // Test sigin in identity and return response with JWT Token
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromForm] RegisterModel signInData, 
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] UserManager<ApplicationUser> userManager
            )
        {
            ThrowIfRegisterModelInvalid(signInData);
            // var user = new ApplicationUser { UserName = signInData.username, Email = signInData.email };
            // await userManager.CreateAsync(user, signInData.password);

            var user = await userManager.FindByEmailAsync(signInData.email);

            if (user == null || user.Id == null) 
            {
                throw new Exception("user invalid");
            }
            var result = await signInManager.PasswordSignInAsync(user, signInData.password, true, false);
            if (!result.Succeeded)
            {
                throw new Exception("login invalid");
            }
            var principal = await signInManager.CreateUserPrincipalAsync(user);


            var jwt = GenerateJWT.Generate(principal.Claims, JWTConfig.Key, "http://localhost", DateTime.Today.AddHours(1));
            return Ok(jwt);
        }
        // Test register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromForm] RegisterModel signInData, 
            [FromServices] UserManager<ApplicationUser> userManager
            )
        {
            ThrowIfRegisterModelInvalid(signInData);
            var user  = new ApplicationUser {
                UserName = signInData.username,
                Email = signInData.email
            };
            var create = await userManager.CreateAsync(user, signInData.password);
            if (!create.Succeeded)
            {
                throw new Exception("Error on create user");
            }
            return Created("register", user.Id.ToString());
        }
    }
}
