namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public class EditRecordCommand : IAddOrEditRecordRequest
    {
        public CardRecord CardRecord { get; set; }
    }
}
