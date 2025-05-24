using MongoDB.Bson;

namespace ACS_Reception.Domain.Entities.CardRecords
{
    public class SickList(ObjectId cardId, DateTime date, string doctor) : CardRecord(cardId, date, doctor)
    {
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public override string GetRecordName => "Sick List";

        public override string GetInfo
        {
            get
            {
                if (EndDate == DateTime.MaxValue)
                {
                    return $"From {Date} to [blank] the person was unable to work/study due to their sickness";
                }
                return $"From {Date} to {EndDate} the person was unable to work/study due to their sickness";
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SickList)
            {
                return false;
            }

            if (!base.Equals(obj) ||
                EndDate.Ticks / (long) 1e7 != (obj as SickList)!.EndDate.Ticks / (long) 1e7)
            {
                return false;
            }

            return true;
        }
    }
}
