namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public class AddRecordHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddRecordCommand>
    {
        public async Task Handle(AddRecordCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.RecordRepository.AddAsync(request.CardRecord, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
