namespace ACS_Reception.Application.CardUseCases.Commands
{
    public class EditCardHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditCardCommand>
    {
        public async Task Handle(EditCardCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.CardRepository.UpdateAsync(request.Card, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
