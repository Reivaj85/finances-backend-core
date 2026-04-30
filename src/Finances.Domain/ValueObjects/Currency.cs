using Finances.Domain.Common;

namespace Finances.Domain.ValueObjects;

public readonly record struct Currency
{
    public string Code { get; }

    private Currency(string code)
    {
        Code = code;
    }

    public static Result<Currency> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<Currency>.Failure(new Error(
                "Currency.Required",
                "La moneda es obligatoria."));
        }

        var normalizedCode = code.Trim().ToUpperInvariant();

        if (normalizedCode.Length != 3 || normalizedCode.Any(character => character < 'A' || character > 'Z'))
        {
            return Result<Currency>.Failure(new Error(
                "Currency.InvalidIsoCode",
                "La moneda debe ser un código ISO de tres letras."));
        }

        return Result<Currency>.Success(new Currency(normalizedCode));
    }

    public override string ToString() => Code;
}
