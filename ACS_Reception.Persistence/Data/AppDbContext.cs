using ACS_Reception.Domain.Entities.CardRecords;
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
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CardRecord>()
                .HasDiscriminator<string>("CardRecordType")
                .HasValue<CardRecord>("CardRecord")
                .HasValue<Check>("Check")
                .HasValue<Analyzis>("Analyzis")
                .HasValue<Prescription>("Prescription")
                .HasValue<SickList>("SickList");
        }
    }
}
