using BadTrip.Domain.ValueObjects;

namespace BadTrip.Domain.Services;

public record PaymentResult(bool Success, string? TransactionId, string? FailureReason);

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(Money amount);
}
