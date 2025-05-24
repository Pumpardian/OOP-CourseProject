using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace ACS_Reception.Domain.Entities
{
    [Collection("Records")]
    public abstract class CardRecord(ObjectId cardId, DateTime date, string doctor) : Entity
    {
        public ObjectId CardId { get; set; } = cardId;
        public DateTime Date { get; set; } = date;
        public string Doctor { get; set; } = doctor;

        public abstract string GetRecordName { get; }
        public abstract string GetInfo { get; }

        public override string ToString()
        {
            return $"{GetRecordName} {Date}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not CardRecord)
            {
                return false;
            }

            if (Id != (obj as CardRecord)!.Id ||
                CardId != (obj as CardRecord)!.CardId ||
                Date.Ticks / (long) 1e7 != (obj as CardRecord)!.Date.Ticks / (long) 1e7 ||
                Doctor != (obj as CardRecord)!.Doctor)
            {
                return false;
            }

            return true;
        }
    }
}
