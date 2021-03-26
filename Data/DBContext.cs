using Microsoft.EntityFrameworkCore;
using doggo.Models;

namespace doggo.Data
{
    public class DBContext : DbContext
    {
        public DBContext (DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<UserDTO> User { get; set; }
    }
}