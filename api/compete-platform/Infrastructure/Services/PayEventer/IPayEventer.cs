namespace compete_platform.Infrastructure.Services.PayEventer
{
    public interface IPayEventer
    {
        public Task<string> PayRequestedEvent(long userId, decimal amount);
        public Task PaySuccessEvent(long userId, decimal amount, 
            string paymentId, string correlationId);
        public Task PayFailedEvent(long userId, decimal amount, 
            string paymentId, string error, string correlationId);
        public Task<string> PayoutRequestedEvent(long userId, decimal amount);
        public Task PayoutSuccessEvent(long userId, decimal amount, 
            string payoutId, string correlationId);
        public Task PayoutFailedEvent(long userId, decimal amount, string payoutId, 
            string error, string correlationId);
    }
}
