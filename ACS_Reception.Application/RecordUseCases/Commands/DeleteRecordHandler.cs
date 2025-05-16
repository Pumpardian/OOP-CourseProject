namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public class DeleteRecordHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteRecordCommand>
    {
        public async Task Handle(DeleteRecordCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.RecordRepository.DeleteAsync(request.CardRecord, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
