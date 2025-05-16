namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public class AddRecordCommand : IAddOrEditRecordRequest
    {
        public CardRecord CardRecord { get; set; }
    }
}
