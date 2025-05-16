namespace ACS_Reception.Application.CardUseCases.Commands
{
    public class AddCardHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddCardCommand>
    {
        public async Task Handle(AddCardCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.CardRepository.AddAsync(request.Card, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
