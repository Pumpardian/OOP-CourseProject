namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public class AddDoctorHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddDoctorCommand>
    {
        public async Task Handle(AddDoctorCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.DoctorRepository.AddAsync(request.Doctor, cancellationToken);
            await unitOfWork.SaveAllAsync();
        }
    }
}
