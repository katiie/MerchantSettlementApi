namespace LC_Banking.Test;

[TestFixture]
public class RetrieveDataTest
{
    private string _sampleUrl;
    private string _badSampleUrl;
    
    [SetUp]
    public void Setup()
    {
        _sampleUrl = "https://simpledebit.gocardless.io/merchants";
        _badSampleUrl = "https://simpledebit.gocardless.io/merchants/bad";
    }
    
    [Test]
    public async Task GetRequest_WithDefault_ShouldReturnList()
    {
        var response = await Program.GetRequest<List<string>>(_sampleUrl); 
        Assert.That(response, Is.Not.Empty);
    }
    
    [Test]
    public async Task GetRequest_WithWrongUrl_ShouldReturnList()
    {
        var response = await Program.GetRequest<List<string>>(_badSampleUrl); 
        Assert.That(response, Is.Null);
    }
}