namespace ACS_Reception.Application.CardUseCases.Queries
{
    public sealed record GetCardByLastNameAndYearQuery(string LastName, int Year) : IRequest<Card>;
}
