namespace ACS_Reception.Application.CardUseCases.Queries
{
    public sealed record GetCardByIdQuery(ObjectId Id) : IRequest<Card>;
}
