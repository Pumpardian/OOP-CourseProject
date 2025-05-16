namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public sealed record DeleteDoctorCommand(Doctor Doctor) : IRequest;
}
