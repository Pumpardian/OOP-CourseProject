using ACS_Reception.Domain.Entities;

namespace ACS_Reception.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<Card> CardRepository { get; }
        IRepository<Doctor> DoctorRepository { get; }
        IRepository<CardRecord> RecordRepository { get; }

        public Task SaveAllAsync();
        public Task DeleteDatabaseAsync();
        public Task CreateDatabaseAsync();
    }
}
