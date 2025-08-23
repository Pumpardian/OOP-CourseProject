namespace ACS_Reception.Domain.Abstractions
{
    public interface IMediator
    {
        public Task Send(IRequest request);

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
    }
}