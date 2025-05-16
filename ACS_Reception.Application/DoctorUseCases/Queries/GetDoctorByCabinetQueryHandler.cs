namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public class GetDoctorByCabinetQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorByCabinetQuery, Doctor>
    {
        public async Task<Doctor> Handle(GetDoctorByCabinetQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DoctorRepository.FirstOrDefaultAsync(d => d.CabinetNumber == request.Cabinet, cancellationToken);
        }
    }
}
