namespace ACS_Reception.Application.CardUseCases.Commands
{
    public interface IAddOrEditCardRequest : IRequest
    {
        Card Card { get; set; }
    }
}
