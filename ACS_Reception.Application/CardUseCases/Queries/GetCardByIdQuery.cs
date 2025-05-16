namespace ACS_Reception.Application.CardUseCases.Queries
{
    public sealed record GetCardByIdQuery(int Id) : IRequest<Card>;
}
