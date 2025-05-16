namespace ACS_Reception.Application.RecordUseCases.Commands
{
    public sealed record DeleteRecordCommand(CardRecord CardRecord) : IRequest;
}
