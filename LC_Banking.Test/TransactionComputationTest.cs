namespace LC_Banking.Test;


public class MainTests
{
    private List<Transaction>? _transactions;
    [SetUp]
    public void Setup()
    {
        _transactions = new List<Transaction>();
        _transactions.Add(new Transaction()
        {
            Amount = 6700,
            Fee = 20
        });
        _transactions.Add(new Transaction()
        {
            Amount = 54869,
            Fee = 290
        });
        _transactions.Add(new Transaction()
        {
            Amount = 50033,
            Fee = 297
        });
    }

    [Test]
    public void Discount_ShouldReturnCorrectAmount()
    {
        float expectedValue = (float)(3000 - (0.04*3000));
        var actualValue = Program.GetDiscountedFeeAmount(3000, 4);
        Assert.AreEqual(actualValue, expectedValue);
    }
    
    [Test]
    public void CalulateTotals_ShouldReturnCorrectAmount()
    {
        (float, float) expectedValue = (111602f, 607f);
        var response = Program.CalculateTotals(_transactions);
        Assert.That(expectedValue, Is.EqualTo(response));
    }

    [Test]
    public void ComputeTotalAmount_ShouldReturnCorrectAmount()
    {
        var expectedValue = 5555f;
        var response = Program.ComputeTotalAmount(50, 5600, 10);
        Assert.That(expectedValue, Is.EqualTo(response));
        
    }
}