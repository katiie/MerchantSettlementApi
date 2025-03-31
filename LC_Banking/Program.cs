using System.Text.Json;
using System.Text.Json.Nodes;

namespace LC_Banking;

public class Program
{
    // Get Merchants
    // For each Merchant, get the transactions
    // For each Merchant, get the total fee
    // For each Merchant, calculate total amount
    // check does merchant meet minimum requirements for discount
    // if yes, apply discount to the total fee 
    // Subtract total fee from total amount
    
    static readonly HttpClient Client = new HttpClient();
    static readonly string GetMerchantsUrl = "https://simpledebit.gocardless.io/merchants";
    static readonly string GetMerchantsTransactionUrl = "https://simpledebit.gocardless.io/merchants";

    public static void Main(string[] args)
    {
        var merchantIds = GetRequest<List<string>>(GetMerchantsUrl).Result;
        if (merchantIds != null && merchantIds.Any())
        {
            var merchantSettlements = new List<MerchantSettlement>();
            foreach (var merchant in merchantIds)
            {
                // Loop through the transactions
                var transactionsResponse =
                    GetRequest<TransactionsResponse>($"{GetMerchantsTransactionUrl}/{merchant}").Result;
                var merchantSettlement = GetMerchantSettlement(transactionsResponse);
                if (merchantSettlement != null)
                    merchantSettlements.Add(merchantSettlement);
            }

            // Print settlement
            PrintMerchantSettlements(merchantSettlements);

            // Build CSV
        }
        else
        {
            Console.WriteLine("There are no merchants.");
        }
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

    public static float GetDiscountedFeeAmount(float feeAmount, int discount) => feeAmount -  feeAmount * (discount / 100f);

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
                var data = JsonSerializer.Deserialize<T>(result);
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