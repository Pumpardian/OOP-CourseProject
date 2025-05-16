namespace ACS_Reception.Application.CardUseCases.Commands
{
    public sealed record DeleteCardCommand(Card card) : IRequest;
}
