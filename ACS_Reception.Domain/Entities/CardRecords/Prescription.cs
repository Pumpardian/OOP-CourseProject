using MongoDB.Bson;

namespace ACS_Reception.Domain.Entities.CardRecords
{
    public class Prescription(ObjectId cardId, DateTime date, string doctor, string medicineName, double discountRate) : CardRecord(cardId, date, doctor)
    {
        public string MedicineName { get; set; } = medicineName;
        public double DiscountRate { get; set; } = discountRate;

        public override string GetRecordName
        {
            get => "Prescription";
        }

        public override string GetInfo
        {
            get
            {
                return $"Medicine: {MedicineName}, with discount {DiscountRate}.";
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Prescription)
            {
                return false;
            }

            if (!base.Equals(obj) ||
                MedicineName != (obj as Prescription)!.MedicineName ||
                DiscountRate != (obj as Prescription)!.DiscountRate)
            {
                return false;
            }

            return true;
        }
    }
}
