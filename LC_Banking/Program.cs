using System.Text.Json;
using System.Text.Json.Nodes;

namespace LC_Banking;

public static class Program
{
    // Get Merchants
    // For each Merchant, get the transactions
    // For each Merchant, get the total fee
    // For each Merchant, calculate total amount
    // check does merchant meet minimum requirements for discount
    // if yes, apply discount to the total fee 
    // Subtract total fee from total amount

    private static readonly HttpClient Client = new HttpClient();
    private static readonly string GetMerchantsUrl = "https://simpledebit.gocardless.io/merchants";
    private static readonly string GetMerchantsTransactionUrl = "https://simpledebit.gocardless.io/merchants";

    public static async Task Main(string[] args)
    {
        var merchantIds = await GetRequest<List<string>>(GetMerchantsUrl);
        if (merchantIds != null && merchantIds.Any())
        {
            Console.WriteLine("Hello world");
            var merchantSettlements = new List<MerchantSettlement>();
            foreach (var merchant in merchantIds)
            {
                // Loop through the transactions
                var transactionsResponse = await GetRequest<TransactionsResponse>($"{GetMerchantsTransactionUrl}/{merchant}");
                var merchantSettlement = GetMerchantSettlement(transactionsResponse);
                if (merchantSettlement != null)
                    merchantSettlements.Add(merchantSettlement);
            }

            // Print settlement
            Console.WriteLine($"count: {merchantSettlements.Count()}");
            PrintMerchantSettlements(merchantSettlements);

            // Build CSV
        }
        else
        {
            Console.WriteLine("There are no merchants.");
        }

        Console.ReadLine();
    }

    public static void PrintMerchantSettlements(List<MerchantSettlement> merchantSettlements)
    {
        foreach (var merchant in merchantSettlements)
        {
            Console.WriteLine($"Merchant: {merchant.Iban} - {merchant.Amount}");
        }
    }

    public static MerchantSettlement? GetMerchantSettlement(TransactionsResponse transactionsResponse)
    {
        if (transactionsResponse == null || transactionsResponse.Transactions == null) return null;
        var (totalAmount, totalFees) = CalculateTotals(transactionsResponse.Transactions.ToList());

        // check if eligible for discount
        var isEligible = transactionsResponse?.Transactions.Count() >=
                         transactionsResponse?.Discount.MinimumTransactionCount;
        if (isEligible)
            totalAmount = ComputeTotalAmount(totalFees, totalAmount, transactionsResponse.Discount.FeesDiscount);

        return new MerchantSettlement { Amount = totalAmount, Iban = transactionsResponse.Iban };

    }
    
    public static float ComputeTotalAmount(float totalFees, float totalAmount, int feesDiscount)
    {
        var adjustedFees = GetDiscountedFeeAmount(totalFees, feesDiscount);
        
        totalAmount -= adjustedFees;
        return totalAmount;
    }

    public static float GetDiscountedFeeAmount(float feeAmount, int discountPercentage)
    {
        // Convert the integer percentage to a proper decimal fraction
        float discountFraction = discountPercentage / 100f;
    
        // Calculate the discounted amount (applying the discount)
        return feeAmount * (1 - discountFraction);
    }
    
    public static (float, float) CalculateTotals(List<Transaction> transactions)
    {
        if (!transactions.Any())
            return (0, 0);

        var totalAmount = transactions.Sum(t => t.Amount);
        var totalFee = transactions.Sum(t => t.Fee);

        return (totalAmount, totalFee);
    }

    public static async Task<T?> GetRequest<T>(string url)
    {
        try
        {
            var response = await Client.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = JsonSerializer.Deserialize<T>(result, options);
                return data;
            }
            else
                return default(T);
        }
        catch (JsonException exception)
        {
            return default(T);
        }
        catch (Exception exception)
        {
            return default(T);
        }
    }
}