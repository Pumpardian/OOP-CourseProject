namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public interface IAddOrEditRecordRequest : IRequest
    {
        CardRecord CardRecord { get; set; }
    }
}
