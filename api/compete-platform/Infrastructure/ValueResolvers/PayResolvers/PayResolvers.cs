using Compete_POCO_Models.Models;
using System.Linq.Expressions;

namespace compete_platform.Infrastructure.ValueResolvers
{
    public static class PayResolvers
    {
        public static PayState[] PayoutStatuses = new PayState[] {
                PayState.RequestPayout,
                PayState.RequestPayoutSuccess,
                PayState.RequestPayoutFailed
            };
        public static PayState[] PaymentStatuses = new PayState[] {
                PayState.TopUpFailed,
                PayState.TopUpSuccess,
                PayState.RequestTopUp
            };
        public static Dictionary<PayState, string> stateToStatus = new()
        { { PayState.RequestPayout, "В процессе"},
            { PayState.RequestTopUp, "В процессе"},
            {PayState.RequestPayoutFailed, "Ошибка" },
            {PayState.TopUpFailed, "Ошибка" },
            {PayState.RequestPayoutSuccess, "Успешно" },
            {PayState.TopUpSuccess, "Успешно" }
        };
        public static Expression<Func<PayEvent, string>> MapEventToStatus => 
            e => stateToStatus[e.PayState];
        public static Expression<Func<PayEvent, string>> GetEventType =>
            e => PaymentStatuses.Contains(e.PayState) ? "Ввод" : "Вывод";
    }
}
