namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public sealed record GetDoctorByIdQuery(int Id) : IRequest<Doctor>;
}
