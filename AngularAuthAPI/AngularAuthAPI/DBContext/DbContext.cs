using AngularAuthAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace AngularAuthAPI.DBContext
{


    namespace AngularAuthAPI.DBContext
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            public DbSet<LoginUser> LoginUsers { get; set; }
            // Use LoginUser instead of LoginUserModel
        }
    }



}
