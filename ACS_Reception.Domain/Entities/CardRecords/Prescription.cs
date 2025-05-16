namespace ACS_Reception.Domain.Entities.CardRecords
{
    public class Prescription(DateTime date, string note, int cardId, string medicineName, double discountRate) : CardRecord(date, note, cardId)
    {
        public string MedicineName { get; } = medicineName;
        public double DiscountRate { get; } = discountRate;

        public override string GetRecordName
        {
            get => "Prescription";
        }
    }
}
