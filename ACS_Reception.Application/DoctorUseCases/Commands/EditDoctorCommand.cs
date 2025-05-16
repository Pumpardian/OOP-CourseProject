namespace ACS_Reception.Application.DoctorUseCases.Commands
{
    public class EditDoctorCommand : IAddOrEditDoctorRequest
    {
        public Doctor Doctor { get; set; }
    }
}
