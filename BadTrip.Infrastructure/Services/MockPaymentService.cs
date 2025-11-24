using BadTrip.Domain.Services;
using BadTrip.Domain.ValueObjects;

namespace BadTrip.Infrastructure.Services;

public class MockPaymentService : IPaymentService
{
    private const decimal MaxAllowedAmount = 10000m;

    public Task<PaymentResult> ProcessPaymentAsync(Money amount)
    {
        if (amount.Amount > MaxAllowedAmount)
        {
            return Task.FromResult(new PaymentResult(
                Success: false,
                TransactionId: null,
                FailureReason: $"Payment amount {amount.Amount} {amount.Currency} exceeds maximum allowed amount of {MaxAllowedAmount}"
            ));
        }

        var transactionId = GenerateTransactionId();

        return Task.FromResult(new PaymentResult(
            Success: true,
            TransactionId: transactionId,
            FailureReason: null
        ));
    }

    private static string GenerateTransactionId()
    {
        return $"TXN-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
    }
}
