using Finances.Domain.Common;

namespace Finances.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            return Result<Money>.Failure(new Error(
                "Money.NegativeAmount",
                "El monto financiero no puede ser negativo."));
        }

        return Result<Money>.Success(new Money(amount, currency));
    }

    public bool IsPositive => Amount > 0;
}
