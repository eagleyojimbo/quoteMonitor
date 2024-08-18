using System.Text.Json;
internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {
            //Obtém o nome do binário:
            string binaryName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine($"Modo de uso:\t./{binaryName} PETR4 22.67 22.59");
        }
        else
        {
            Console.WriteLine("Parâmetro\tValor");
            Console.WriteLine($"Cotação:\t{args[0]}");
            Console.WriteLine($"Valor Máximo:\t{args[1]}");
            Console.WriteLine($"Valor Mínimo:\t{args[2]}");
            
            //Definir o endereço para mandarmos a requisição:
            string url = $"https://brapi.dev/api/quote/{args[0]}?range=1d&interval=1d&token=g5ZNSxJyiUGCHmRLoKHHYn";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                //Get the data from response
                string jsonString = await response.Content.ReadAsStringAsync();

                //Get Current Stock Price
                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = document.RootElement;
                    JsonElement resultsArray = root.GetProperty("results");

                    if (resultsArray.GetArrayLength() <= 0)
                    {
                        return;
                    }
                    JsonElement firstResult = resultsArray[0];
                    double regularMarketPrice = firstResult.GetProperty("regularMarketPrice").GetDouble();
                    Console.WriteLine($"Preço Atual:\t{regularMarketPrice}");
                }
            }
        }
    }
}