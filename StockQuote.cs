public class StockQuote
{
    public string Symbol { get; init; } // Propriedade para pegar o símbolo da ação informado inicialmente
    public double RegularMarketPrice { get; set; } // Propriedade para armazenar o preço atual da ação

    public StockQuote(string symbol, double regularMarketPrice) // Construtor para inicializar a classe com o símbolo e preço da cotação
    {
        Symbol = symbol;
        RegularMarketPrice = regularMarketPrice;
    }

    public override string ToString()
    {
        return $"{Symbol}: {RegularMarketPrice}";
    }
}
