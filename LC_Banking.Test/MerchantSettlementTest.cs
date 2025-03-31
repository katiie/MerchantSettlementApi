namespace LC_Banking.Test;

[TestFixture]
public class MerchantSettlementTest
{
    private TransactionsResponse _transactionsResponse;

    [SetUp]
    public void Setup()
    {
        _transactionsResponse = new()
        {
            Id = "1",
            Iban = "123",
            Discount = new Discount()
            {
                FeesDiscount = 5,
                MinimumTransactionCount = 1,
            },
            Transactions =
            [
                new Transaction()
                {
                    Amount = 12000,
                    Fee = 200
                },

                new Transaction()
                {
                    Amount = 6000,
                    Fee = 10
                },
            ]
        };
    }

    [Test]
    public void GetMerchantSettlement_WithDiscountableTransaction_ShouldReturnObject()
    {
        var response = Program.GetMerchantSettlement(_transactionsResponse);
        var discountedFee = Program.GetDiscountedFeeAmount(
            _transactionsResponse.Transactions.Sum(transaction => transaction.Fee),
            _transactionsResponse.Discount.FeesDiscount);
        var transactionAmount = _transactionsResponse.Transactions.Sum(transaction => transaction.Amount);
        float expectedFee = transactionAmount - discountedFee;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Iban, Is.EqualTo(_transactionsResponse.Iban));
        Assert.That(response.Amount, Is.EqualTo(expectedFee));
    }
    
    [Test]
    public void GetMerchantSettlement_WithNoDiscountableTransaction_ShouldReturnObject()
    {
        _transactionsResponse.Discount.MinimumTransactionCount = 5;
        var response = Program.GetMerchantSettlement(_transactionsResponse);
        var expectedFee = _transactionsResponse.Transactions.Sum(transaction => transaction.Amount);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Iban, Is.EqualTo(_transactionsResponse.Iban));
        Assert.That(response.Amount, Is.EqualTo(expectedFee));
    }
    
    [Test]
    public void GetMerchantSettlement_WithNoTransaction_ShouldReturnNull()
    {
        var response = Program.GetMerchantSettlement(new TransactionsResponse());
        Assert.That(response, Is.Null);
    }
    
}