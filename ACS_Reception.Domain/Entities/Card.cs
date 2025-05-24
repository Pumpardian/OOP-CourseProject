using MongoDB.EntityFrameworkCore;

namespace ACS_Reception.Domain.Entities
{
    [Collection("Cards")]
    public class Card : Entity
    {
        public string FirstName { get; set; } = "First Name";
        public string LastName { get; set; } = "Last Name";
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        public IReadOnlyList<CardRecord> CardRecords { get => cardRecords.AsReadOnly(); }
        private List<CardRecord> cardRecords = [];

        public Card() { }

        public Card(string firstName, string lastName, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public override string ToString()
        {
            return $"{LastName} {FirstName} {BirthDate.Year}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Card)
            {
                return false;
            }

            if (Id != (obj as Card)!.Id ||
                FirstName != (obj as Card)!.FirstName ||
                LastName != (obj as Card)!.LastName ||
                BirthDate.Ticks / (long) 1e7 != (obj as Card)!.BirthDate.Ticks / (long) 1e7)
            {
                return false;
            }

            return true;
        }
    }
}
