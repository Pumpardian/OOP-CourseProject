namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public sealed record GetDoctorsQuery : IRequest<IEnumerable<Doctor>>;
}
