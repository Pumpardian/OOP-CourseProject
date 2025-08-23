namespace ACS_Reception.Application.DoctorUseCases.Queries
{
    public sealed record GetDoctorByIdQuery(ObjectId Id) : IRequest<Doctor>;
}
