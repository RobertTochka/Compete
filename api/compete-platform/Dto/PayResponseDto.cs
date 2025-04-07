using Yandex.Checkout.V3;

namespace compete_platform.Dto
{
    public class PayResponseDto
    {
        public Confirmation Confirmation { get; set; } = null!;
        public string? Text { get; set; }
    }
}
