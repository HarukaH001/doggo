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
        public DbSet<ItemDTO> Item { get; set; }
        public DbSet<ItemStockDTO> ItemStock { get; set; }
        public DbSet<ReservationRecordDTO> ReservationRecord { get; set; }
        public DbSet<TimeTableQuery> TimeTable { get; set; }
    }
}