using ErpSystem.Domain.Exceptions;
using ErpSystem.Domain.ValueObjects;
using Xunit;

namespace ErpSystem.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_ValidAmountAndCurrency_ReturnsMoney()
    {
        var money = new Money(100, "USD");
        Assert.Equal(100, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Create_NegativeAmount_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Money(-1, "USD"));
    }

    [Fact]
    public void Create_InvalidCurrency_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Money(100, "INVALID"));
    }

    [Fact]
    public void Add_SameCurrency_ReturnsCorrectSum()
    {
        var a = new Money(100, "USD");
        var b = new Money(50, "USD");
        var result = a.Add(b);
        Assert.Equal(150, result.Amount);
    }

    [Fact]
    public void Add_DifferentCurrency_ThrowsDomainException()
    {
        var a = new Money(100, "USD");
        var b = new Money(50, "EUR");
        Assert.Throws<DomainException>(() => a.Add(b));
    }

    [Fact]
    public void Zero_ReturnsMonyWithZeroAmount()
    {
        var money = Money.Zero("USD");
        Assert.Equal(0, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Equals_SameAmountAndCurrency_ReturnsTrue()
    {
        var a = new Money(100, "USD");
        var b = new Money(100, "USD");
        Assert.Equal(a, b);
    }
}
