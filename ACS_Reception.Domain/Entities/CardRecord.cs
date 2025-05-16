using ACS_Reception.Domain.Abstractions;

namespace ACS_Reception.Domain.Entities
{
    public abstract class CardRecord(DateTime date, string note, int cardId) : Entity
    {
        public DateTime Date { get; } = date;
        public string Note { get; } = note;
        public int CardId { get; } = cardId;

        public abstract string GetRecordName { get; }
    }
}
