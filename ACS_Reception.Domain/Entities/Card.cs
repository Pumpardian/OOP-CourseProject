using ACS_Reception.Domain.Abstractions;

namespace ACS_Reception.Domain.Entities
{
    public class Card(string firstName, string lastName, DateOnly birthDate) : Entity
    {
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public DateOnly BirthDate { get; } = birthDate;
        public List<CardRecord> CardRecords { get; } = [];
    }
}
