using compete_platform.Dto;
using Yandex.Checkout.V3;

namespace compete_platform.Infrastructure.Services.PaymentService
{
    public interface IPaymentService
    {
        public Task<PayResponseDto> CreatePaymentAsync(PayRequestDto req);
        public Task HandlePayNotification(Notification notification);
        public Task CreatePayoutAsync(PayoutRequest req, string userId);
        public  Task HandleSuccessPayment(Payment payment);
    }
}
