namespace ACS_Reception.Domain.Entities.CardRecords.Analyzises
{
    public class UrineAnalyzis(DateOnly date, string note, Guid cardId) : Analyzis(date, note, cardId)
    {
    }
}
