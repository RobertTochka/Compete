using compete_platform.Dto;
using compete_platform.Infrastructure.Services.PayEventer;
using compete_platform.Infrastructure.Services.PayRepository;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Services;
using Compete_POCO_Models.Infrastrcuture.Data;
using Yandex.Checkout.V3;

namespace compete_platform.Infrastructure.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IPayEventer _eventer;
        private readonly CPayRepository _paySrc;
        private readonly AppConfig _cfg;
        private readonly IUserService _userProvider;
        public static string Correlation => nameof(Correlation);
        private static string Label = "UserId";
        private static string[] variants = new[] {"bank-card","ymoney","sberpay", "sbp", "balance-phone" };
        public PaymentService(ILogger<PaymentService> logger, 
            AppConfig cfg,
            IUserService userProvider,
            CPayRepository paySrc,
            IPayEventer eventer) 
        {
            _logger = logger;
            _eventer = eventer;
            _paySrc = paySrc;
            _cfg = cfg;
            _userProvider = userProvider;
        }
        private PaymentMethod GetPaymentMethodData(PayRequestDto payRequest)
        {
            PaymentMethod result = payRequest.Variant switch
            {
                "bank-card" => new() {Type = PaymentMethodType.BankCard },
                "ymoney" => new() { Type = PaymentMethodType.YooMoney },
                "sberpay" => new() { Type= PaymentMethodType.Sberbank },
                "sbp" => new() { Type = PaymentMethodType.Sbp},
                "balance-phone" => new() { Type = PaymentMethodType.MobileBalance, Phone = payRequest.Identifier},
                _ => throw new InvalidProgramException()
            };
            return result;
        }
        private Confirmation GetPaymentConfirmation(PayRequestDto payRequest)
        {
            Confirmation result = payRequest.Variant switch
            {
                "balance-phone" => new() { Type = ConfirmationType.External },
                _ => new()
                {
                    Type = ConfirmationType.Redirect,
                    ReturnUrl = $"{_cfg.Host}/" +
                $"balance?fromShop={AppDictionary.ThanksForTopUp}"
                }
            };
            return result;
        }
        public async Task<PayResponseDto> CreatePaymentAsync(PayRequestDto req)
        {
            var userId = req.UserId!;
            var amount = req.Amount;
            if (!variants.Contains(req.Variant))
                throw new ApplicationException(AppDictionary.NotSupportedPayment);
            var _client = new Client(_cfg.ShopId, _cfg.PaymentKey).MakeAsync();
            _logger.LogInformation(
                $"Пришел запрос на пополнение баланса от пользователя с userId - [{userId}]");
            var correlationId = await _eventer.PayRequestedEvent(long.Parse(userId), amount);
            var newPayment = new NewPayment
            {
                PaymentMethodData = GetPaymentMethodData(req),
                Amount = new Amount { Value = amount, Currency = "RUB" },
                Confirmation = GetPaymentConfirmation(req),
                Capture = true,
                Metadata = new() { { Label, userId }, { Correlation, correlationId} },
            };
            Payment payment = await _client.CreatePaymentAsync(newPayment);
            _logger.LogInformation($"Пользователь с id - [{userId}] был отправлен на страницу подтверждения");
            return new() 
            { 
                Confirmation =  payment.Confirmation,
                Text = payment.Confirmation.Type == ConfirmationType.External 
                ?  AppDictionary.ConfirmOnPhone : null
            };
        }
        public async Task HandleSuccessPayment(Payment payment)
        {
            try
            {
                var userId = long.Parse(payment.Metadata[Label]);
                var correlationId = payment.Metadata[Correlation];
                await _userProvider.TopUpUserBalance(
                    payment.Amount.Value, userId, payment.Id, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось добавить событие об успешной оплате в журнал логов\n" +
                    $"{ex.Message}");
            }
        }
        private async Task HandlePaymentCapture(Payment payment)
        {
            var userId = long.Parse(payment.Metadata[Label]);

            if (payment.Paid)
            {
                var _client = new Client(_cfg.ShopId, _cfg.PaymentKey).MakeAsync();
                try
                {
                 
                    _logger.LogInformation($"Пришел запрос на потдверждение оплаты от нашего " +
                        $"сервиса для пользователя [{userId}]");
                    await _client.CapturePaymentAsync(payment.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Возникла ошибка при подтверждении платежа\n, {ex.Message}");
                }
            }
        }
        private async Task HandleSuccessPayout(Payout payout)
        {
            try
            {
                _logger.LogInformation($"Payout: {payout.Id}");
                var userId = long.Parse(payout.Metadata[Label]);
                var correlationId = payout.Metadata[Correlation];
                _logger.LogInformation($"User: {userId}");
                await _eventer.PayoutSuccessEvent(userId, payout.Amount.Value, payout.Id, correlationId);
                _logger.LogInformation("Sucess");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось зафиксировать событие об успешной выплате\n" +
                    $"{ex.Message}");
            }
        }
        private async Task HandleCanceledPayout(Payout payout)
        {
            try
            {
                var userId = long.Parse(payout.Metadata[Label]);
                var correlationId = payout.Metadata[Correlation];
                await _userProvider.TopUpUserBalance(payout.Amount.Value, 
                    userId, payout.Id, 
                    correlationId,
                    payout.CancellationDetails.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось зафиксировать событие об отмене выплаты\n" +
                    $"{ex.Message}");
            }
        }
        public async Task HandlePayNotification(Notification notification)
        {
            if (notification is PaymentWaitingForCaptureNotification paymentWaitingForCaptureNotification)
                await HandlePaymentCapture(paymentWaitingForCaptureNotification.Object);
            else if (notification is PaymentSucceededNotification paymentSucceededNotification)
                await HandleSuccessPayment(paymentSucceededNotification.Object);
            else if (notification is PaymentCanceledNotification paymentCanceledNotification)
                await HandleCanceledPayment(paymentCanceledNotification.Object);
            else if (notification is PayoutSucceededNotification payoutSucceededNotification)
                await HandleSuccessPayout(payoutSucceededNotification.Object);
            else if (notification is PayoutCanceledNotification payoutCanceledNotification)
                await HandleCanceledPayout(payoutCanceledNotification.Object);
            else
                _logger.LogWarning("Тип уведомления не был обработан корректно");

        }

        private async Task HandleCanceledPayment(Payment @object)
        {
            try
            {
                var userId = long.Parse(@object.Metadata[Label]);
                var correlationId = @object.Metadata[Correlation];
                await _eventer.PayFailedEvent(userId, @object.Amount.Value, 
                    @object.Id, @object.CancellationDetails.Reason, correlationId);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Не удалось зафиксировать ивент отмены оплаты\n" +
                    $"{ex.Message}");
            }
        }

        public async Task CreatePayoutAsync(PayoutRequest req, string userId)
        {
            _logger.LogInformation($"Пришел запрос на снятие средств пользователем {userId}");
            var parsedUserId = long.Parse(userId);
            var correlationId = await _userProvider.WithdrawalFunds(parsedUserId, req.Amount);
            var _client = new Client(_cfg.AgentId, _cfg.PayoutKey).MakeAsync();
            var payout = new NewPayout()
            {
                Description = $"Вывод средств пользователем {userId}",
                Amount = new()
                {
                    Value = req.Amount + 00.01M,
                    Currency = "RUB"
                },
                PayoutToken = req.Identifier,
                Metadata = new Dictionary<string, string>() 
                { 
                    { Label, userId }, 
                    { Correlation, correlationId} 
                }
            };
            _logger.LogInformation($"Пользователь - [{userId}] ---> Делаю запрос на снятие в юкассу");
            var result = await _client.CreatePayoutAsync(payout);
            _logger.LogInformation($"Запрос на вывод средств от {userId} Завершился успехом");
        }
    }
}
