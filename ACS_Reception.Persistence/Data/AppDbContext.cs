using Microsoft.EntityFrameworkCore;

namespace ACS_Reception.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        DbSet<CardRecord> CardRecords { get; }
        DbSet<Card> Cards { get; }
        DbSet<Doctor> Doctors { get; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
