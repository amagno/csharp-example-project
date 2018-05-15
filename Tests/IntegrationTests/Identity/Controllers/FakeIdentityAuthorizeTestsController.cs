using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
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

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
