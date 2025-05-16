namespace ACS_Reception.Application.CardUseCases.Queries
{
    public sealed record GetCardsQuery : IRequest<IEnumerable<Card>>;
}
