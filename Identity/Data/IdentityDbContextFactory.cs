using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Identity.Data
{

    namespace CsharpDDD.Data.Context
    {
        public class IdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
        {
            private static DbContextOptionsBuilder<ApplicationIdentityDbContext> GetOptionsBuilder { 
                get {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
                    optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
                    
//                  --- add logger
//                  optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] {
//                      new ConsoleLoggerProvider((category, level) => true, true)
//                  }));
                    
                    return optionsBuilder;
                }
            }
            public ApplicationIdentityDbContext CreateDbContext(string[] args)
            {
                return new ApplicationIdentityDbContext(GetOptionsBuilder.Options);
            }
        }
    }
}