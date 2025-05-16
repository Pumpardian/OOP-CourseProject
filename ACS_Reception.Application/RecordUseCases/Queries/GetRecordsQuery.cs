namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public sealed record GetRecordsQuery : IRequest<IEnumerable<CardRecord>>;
}
