namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public class EditDoctorHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditDoctorCommand>
    {
        public async Task Handle(EditDoctorCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.DoctorRepository.UpdateAsync(request.Doctor, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
