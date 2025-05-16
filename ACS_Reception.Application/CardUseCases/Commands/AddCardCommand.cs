namespace ACS_Reception.Application.CardUseCases.Commands
{
    public class AddCardCommand : IAddOrEditCardRequest
    {
        public Card Card { get; set; }
    }
}
