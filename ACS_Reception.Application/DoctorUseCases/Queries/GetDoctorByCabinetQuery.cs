namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public sealed record GetDoctorByCabinetQuery(int Cabinet) : IRequest<Doctor>;
}
