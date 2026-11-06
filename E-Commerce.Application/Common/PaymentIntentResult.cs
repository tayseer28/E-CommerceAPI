namespace E_Commerce.Application.Common
{
    public sealed class PaymentIntentResult
    {
        public PaymentIntentResult(string paymentIntentId, string clientSecret)
        {
            PaymentIntentId = paymentIntentId;
            ClientSecret = clientSecret;
        }

        public string PaymentIntentId { get; } = default!;
        public string ClientSecret { get; } = default!;
    }

}
