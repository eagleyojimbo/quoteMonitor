using System.Text.Json;

public class StockQuoteFetcher
{
    private readonly HttpClient _httpClient;

    public StockQuoteFetcher()
    {
        _httpClient = new HttpClient();
    }

    public async Task<StockQuote> FetchQuoteAsync(string symbol)
    {
        string url = $"https://brapi.dev/api/quote/{symbol}?range=1d&interval=1d&token=g5ZNSxJyiUGCHmRLoKHHYn";
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string jsonString = await response.Content.ReadAsStringAsync();

        using (JsonDocument document = JsonDocument.Parse(jsonString))
        {
            JsonElement root = document.RootElement;
            JsonElement resultsArray = root.GetProperty("results");

            if (resultsArray.GetArrayLength() <= 0)
            {
                throw new Exception("No results found");
            }

            JsonElement firstResult = resultsArray[0];
            double regularMarketPrice = firstResult.GetProperty("regularMarketPrice").GetDouble();

            return new StockQuote(symbol, regularMarketPrice);
        }
    }
}
