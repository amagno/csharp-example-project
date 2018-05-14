using System;
using Xunit;
using Domain.Entities;


namespace Tests.Domain
{
    public class UserUnitTest
    {
        

        [Fact]
        public void TestAddProject()
        {
            var user = new User("test", "test@email.com");

            user.AddProject(DateTime.Now, DateTime.Now.AddDays(10), "test", "test");
            
            Assert.True(user.Projects.Count.Equals(1));
        }

        [Fact]
        public void TestAddProjectInvalidIntesectionDate()
        {
            var user = new User("test", "test@email.com");
            
            user.AddProject(DateTime.Now, DateTime.Now.AddDays(10), "test", "test");
            user.AddProject(DateTime.Now.AddDays(12), DateTime.Now.AddDays(15), "test2", "test2");
            
            void act1 () => user.AddProject(DateTime.Now.AddDays(2), DateTime.Now.AddDays(8), "test3", "test3");
            void act2 () => user.AddProject(DateTime.Now.AddDays(11), DateTime.Now.AddDays(14), "test2", "test2");
            void act3 () => user.AddProject(DateTime.Now.AddDays(11), DateTime.Now.AddDays(17), "test2", "test2");
                
            Assert.Throws<Exception>((Action)act1);
            Assert.Throws<Exception>((Action)act2);
            Assert.Throws<Exception>((Action)act3);
            Assert.True(user.Projects.Count.Equals(2));
        }
        
        
    }
}