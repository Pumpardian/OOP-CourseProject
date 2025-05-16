namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public sealed record GetDoctorByNameQuery(string LastName, string firstName) : IRequest<Doctor>;
}
