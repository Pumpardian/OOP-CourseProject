using ACS_Reception.Persistence.Data;
using ACS_Reception.Persistence.Repository;

namespace ACS_Reception.Persistence.UnitOfWork
{
    public class EfUnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private readonly AppDbContext context = context;
        private readonly Lazy<IRepository<Card>> cardRepository = new(() => new EfRepository<Card>(context));
        private readonly Lazy<IRepository<CardRecord>> recordRepository = new(() => new EfRepository<CardRecord>(context));
        private readonly Lazy<IRepository<Doctor>> doctorRepository = new(() => new EfRepository<Doctor>(context));

        public IRepository<Card> CardRepository => cardRepository.Value;
        public IRepository<CardRecord> RecordRepository => recordRepository.Value;
        public IRepository<Doctor> DoctorRepository => doctorRepository.Value;

        public async Task CreateDatabaseAsync() => await context.Database.EnsureCreatedAsync();
        public async Task DeleteDatabaseAsync() => await context.Database.EnsureDeletedAsync();
        public async Task SaveAllAsync() => await context.SaveChangesAsync();
    }
}
