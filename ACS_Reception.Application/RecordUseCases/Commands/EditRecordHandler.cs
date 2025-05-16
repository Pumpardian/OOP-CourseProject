namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public class EditRecordHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditRecordCommand>
    {
        public async Task Handle(EditRecordCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.RecordRepository.UpdateAsync(request.CardRecord, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
