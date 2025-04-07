using compete_platform.Infrastructure.Services.PayEventsRepository;
using compete_platform.Infrastructure.Services.PayRepository;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
namespace compete_platform.Infrastructure.Services.PayEventer
{
    public class PayEventer : IPayEventer
    {
        private CPayEventsRepository _eventsSrc;
        private readonly CPayRepository _userPays;

        public PayEventer(CPayEventsRepository eventsSrc, CPayRepository userPays) 
        { 
            _eventsSrc = eventsSrc;
            _userPays = userPays;
        }
        private async Task SavePayEvent(PayEvent e)
        {
            await _eventsSrc.CreateEvent(e);
            await _eventsSrc.SaveChangesAsync();
        }
        public async Task PayFailedEvent(
            long userId, decimal amount, string paymentId, string error, string correlationId)
        {
            var failedPayEvent = new PayEvent()
            {
                Amount = amount,
                PaymentId = paymentId,
                Error = error,
                UserId = userId,
                CreatedUtc = DateTime.UtcNow,
                PayState = PayState.TopUpFailed,
                CorrelationId = correlationId
            };
           await SavePayEvent(failedPayEvent);
        }

        public async Task PayoutFailedEvent(
            long userId, decimal amount, string payoutId, string error, string correlationId)
        {
            var payoutFailedEvent = new PayEvent()
            {
                Amount = amount,
                PaymentId = payoutId,
                Error = error,
                CreatedUtc = DateTime.UtcNow,
                UserId = userId,
                PayState = PayState.RequestPayoutFailed,
                CorrelationId = correlationId
            };
            await SavePayEvent(payoutFailedEvent);
        }

        public async Task<string> PayoutRequestedEvent(long userId, decimal amount)
        {
            var correlationId = Guid.NewGuid().ToString();
            var payoutRequestEvent = new PayEvent()
            {
                Amount = amount,
                CreatedUtc = DateTime.UtcNow,
                UserId = userId,
                PayState = PayState.RequestPayout,
                CorrelationId = correlationId
            };
           await SavePayEvent(payoutRequestEvent);
            return correlationId;
        }
        
        public async Task PayoutSuccessEvent(
            long userId, decimal amount, string payoutId, string correlationId)
        {
            var payOutSuccessEvent = new PayEvent()
            {
                Amount = amount,
                UserId = userId,
                CreatedUtc = DateTime.UtcNow,
                PaymentId = payoutId,
                PayState = PayState.RequestPayoutSuccess,
                CorrelationId = correlationId
            };
            await _userPays.CreatePayAsync(new()
            {
                Amount = amount,
                UserId = userId,
                CreationTime = DateTime.UtcNow,
                Description = AppDictionary.WithdrawalOfFunds
            });
           await SavePayEvent(payOutSuccessEvent);
        }

        public async Task<string> PayRequestedEvent(long userId, decimal amount)
        {
            var correlationId = Guid.NewGuid().ToString();
            var payRequestEvnrt = new PayEvent()
            {
                Amount = amount,
                CreatedUtc = DateTime.UtcNow,
                UserId = userId,
                PayState = PayState.RequestTopUp,
                CorrelationId = correlationId
            };
            await SavePayEvent(payRequestEvnrt);
            return correlationId;
        }

        public async Task PaySuccessEvent(
            long userId, decimal amount, string paymentId, string correlationId)
        {
            var paymentSuccessEvent = new PayEvent()
            {
                UserId = userId,
                CreatedUtc = DateTime.UtcNow,
                Amount = amount,
                PayState = PayState.TopUpSuccess,
                PaymentId = paymentId,
                CorrelationId=correlationId
            };
            await _userPays.CreatePayAsync(new()
            {
                UserId = userId,
                Amount = amount,
                Description = AppDictionary.TopUp,
                CreationTime = DateTime.UtcNow
            });
            await SavePayEvent(paymentSuccessEvent);
        }
    }
}
