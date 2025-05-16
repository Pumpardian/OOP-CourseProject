namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public class AddDoctorCommand : IAddOrEditDoctorRequest
    {
        public Doctor Doctor { get; set; }
    }
}
