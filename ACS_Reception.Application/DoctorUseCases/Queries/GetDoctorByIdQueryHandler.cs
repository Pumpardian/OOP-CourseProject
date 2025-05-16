namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public class GetDoctorByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorByIdQuery, Doctor>
    {
        public async Task<Doctor> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DoctorRepository.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        }
    }
}
