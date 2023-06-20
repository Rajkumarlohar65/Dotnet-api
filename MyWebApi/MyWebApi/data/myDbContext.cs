using Microsoft.EntityFrameworkCore;
using myWebApi.models;

namespace myWebApi.data
{
    public class MyDbContext : DbContext
    {

        public MyDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }
    }
}