namespace ACS_Reception.Application.CardUseCases.Commands
{
    public class DeleteCardHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCardCommand>
    {
        public async Task Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.CardRepository.DeleteAsync(request.card, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
