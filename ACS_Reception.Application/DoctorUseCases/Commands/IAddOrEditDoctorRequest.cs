namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public interface IAddOrEditDoctorRequest : IRequest
    {
        Doctor Doctor { get; set; }
    }
}
