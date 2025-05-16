namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public sealed record GetRecordByIDQuery(int Id) : IRequest<CardRecord>;
}
