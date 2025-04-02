using System.Text.Json;
using System.Text.Json.Serialization;

namespace LC_Banking;

public class TransactionsResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    public string Iban { get; set; }
    public Discount Discount { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
}

public class Discount
{
    [JsonPropertyName("minimum_transaction_count")]
    public int MinimumTransactionCount { get; set; }
    [JsonPropertyName("fees_discount")]
    public int FeesDiscount { get; set; }
}

public class Transaction
{
    // Todo: read on Decimal/Double/Float
    public float Amount { get; set; }
    public float Fee { get; set; }
}


public class MerchantSettlement
{
    public string Iban { get; set; }

    public float Amount { get; set; }
}
