namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public sealed record GetRecordsByCardIdQuery(ObjectId CardId) : IRequest<IEnumerable<CardRecord>>;
}
