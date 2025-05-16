namespace ACS_Reception.Application.CardUseCases.Commands
{
    public class EditCardCommand : IAddOrEditCardRequest
    {
        public Card Card { get; set; }
    }
}
