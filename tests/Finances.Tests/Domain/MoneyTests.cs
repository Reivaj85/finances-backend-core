using Finances.Domain.ValueObjects;

namespace Finances.Tests.Domain;

public sealed class MoneyTests
{
    [Fact]
    public void Create_ShouldRejectNegativeAmount_WhenAmountIsBelowZero()
    {
        var currency = Currency.Create("COP").Value;

        var result = Money.Create(-1, currency);

        Assert.True(result.IsFailure);
        Assert.Equal("Money.NegativeAmount", result.Error?.Code);
        Assert.Contains("negativo", result.Error?.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("CO")]
    [InlineData("COP1")]
    [InlineData("12A")]
    public void Create_ShouldRejectInvalidCurrency_WhenCodeIsNotIsoThreeLetters(string invalidCode)
    {
        var result = Currency.Create(invalidCode);

        Assert.True(result.IsFailure);
        Assert.StartsWith("Currency.", result.Error?.Code, StringComparison.Ordinal);
        Assert.Contains("moneda", result.Error?.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldNormalizeCurrency_WhenCodeIsLowercase()
    {
        var currency = Currency.Create("cop");
        var money = Money.Create(120_000m, currency.Value);

        Assert.True(currency.IsSuccess);
        Assert.True(money.IsSuccess);
        Assert.Equal(120_000m, money.Value.Amount);
        Assert.Equal("COP", money.Value.Currency.Code);
    }
}
