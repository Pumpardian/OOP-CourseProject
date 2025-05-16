namespace ACS_Reception.Domain.Entities.CardRecords
{
    public abstract class Analyzis(DateTime date, string note, int cardId) : CardRecord(date, note, cardId)
    {
        public int Id { get; } = cardId;
        public string Note { get; set; } = note;

        public override string GetRecordName
        {
            get => "Analyzis";
        }
    }
}
