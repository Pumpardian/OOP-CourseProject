namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public class DeleteDoctorHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteDoctorCommand>
    {
        public async Task Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.DoctorRepository.DeleteAsync(request.Doctor, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
