namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public class GetDoctorByNameQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorByNameQuery, Doctor>
    {
        public async Task<Doctor> Handle(GetDoctorByNameQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DoctorRepository.FirstOrDefaultAsync(d => d.FirstName == request.firstName && d.LastName == request.LastName, cancellationToken);
        }
    }
}
