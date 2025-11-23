using BadTrip.Domain.Exceptions;

namespace BadTrip.Domain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ValidationException("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(currency))
                throw new ValidationException("Currency is required.");

            if (currency.Length != 3)
                throw new ValidationException("Currency must be a 3-letter ISO code (e.g., USD, EUR).");

            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new DomainException($"Cannot add money with different currencies: {left.Currency} and {right.Currency}");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator *(Money money, int multiplier)
        {
            if (multiplier < 0)
                throw new ValidationException("Multiplier cannot be negative.");

            return new Money(money.Amount * multiplier, money.Currency);
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
