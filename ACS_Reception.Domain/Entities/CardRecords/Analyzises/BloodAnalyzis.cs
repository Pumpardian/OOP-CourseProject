
namespace ACS_Reception.Domain.Entities.CardRecords.Analyzises
{
    public class BloodAnalyzis(DateOnly date, string note, Guid cardId) : Analyzis(date, note, cardId)
    {
    }
}
