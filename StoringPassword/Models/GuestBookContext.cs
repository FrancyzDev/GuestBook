using Microsoft.EntityFrameworkCore;

namespace StoringPassword.Models
{
    public class GuestBookContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public GuestBookContext(DbContextOptions<GuestBookContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}