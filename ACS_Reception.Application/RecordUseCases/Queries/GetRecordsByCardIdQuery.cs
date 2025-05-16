namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public sealed record GetRecordsByCardIDQuery(int CardId) : IRequest<IEnumerable<CardRecord>>;
}
