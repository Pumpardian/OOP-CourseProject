namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public class GetDoctorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDoctorsQuery, IEnumerable<Doctor>>
    {
        public async Task<IEnumerable<Doctor>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DoctorRepository.ListAllAsync(cancellationToken);
        }
    }
}
