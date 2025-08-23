namespace ACS_Reception.Application.RecordUseCases.Queries
{
    public sealed record GetRecordByIdQuery(ObjectId Id) : IRequest<CardRecord>;
}
