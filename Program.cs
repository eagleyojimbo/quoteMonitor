internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {   
            // Obtém o nome do binário em execução. Semelhante ao argv[0] do C
            string binaryName = AppDomain.CurrentDomain.FriendlyName;
            
            // Printar a maneira correta de executar o binário, pro caso do usuário executar sem passar os argumentos
            Console.WriteLine($"Modo de uso: ./{binaryName} PETR4 22.67 22.59");
            Console.WriteLine("\nDescrição da Aplicação:");
            Console.WriteLine("Esta aplicação monitora a cotação de uma ação específica e, com base em valores limite definidos, envia notificações por email para aconselhar a compra ou venda da ação.");
            Console.WriteLine("Argumentos esperados:");
            Console.WriteLine("1. Símbolo da Ação: O símbolo da ação que deseja monitorar (por exemplo, \"PETR4\").");
            Console.WriteLine("2. Valor Máximo: O valor máximo aceitável para a cotação da ação, acima do qual uma notificação de venda será enviada.");
            Console.WriteLine("3. Valor Mínimo: O valor mínimo aceitável para a cotação da ação, abaixo do qual uma notificação de compra será enviada.");
            Console.WriteLine("\nPor favor, forneça todos os argumentos necessários.");
            Console.WriteLine("Para obter a lista de possíveis cotações, acesse: https://brapi.dev/quotes");
            Console.WriteLine("\nEste programa também depende de um arquivo de configuração chamado \"emailsettings.json\" localizado no mesmo diretório do executável. Este arquivo deve conter:");
            Console.WriteLine("\t- DestinationEmail: Email de destino para as notificações;");
            Console.WriteLine("\t- SmtpServer: O servidor SMTP em que a aplicação vai se autenticar;");
            Console.WriteLine("\t- SmtpPort: A porta em que o serviço SMTP está escutando no servidor;");
            Console.WriteLine("\t- SmtpUser: O usuário com o qual a aplicação vai se autenticar;");
            Console.WriteLine("\t- SmtpPassword: Senha do usuário encodada em base64");
            Console.WriteLine("Por favor, forneça todos os argumentos necessários e verifique se o arquivo de configuração está presente e correto.");
            return;
        }

        //Definir caminho do arquivo de configuração
        string assemblyFilePath = AppDomain.CurrentDomain.BaseDirectory;
        string[] assemblySplitPath = assemblyFilePath.Split(new string[] { "/bin/Debug/" },StringSplitOptions.None);
        string configFilePath = $"{assemblySplitPath[0]}/emailsettings.json";
        
        // Leitura dos argumentos passados pelo usuário
        string symbol = args[0]; // Primeiro argumento: símbolo da ação (ITSA4, PETR4, ...)
        double maxValue = double.Parse(args[1]);// Segundo argumento: Valor Máximo de Preço
        double minValue = double.Parse(args[2]);// Terceiro argumento: Valor Mínimo de Preço

        // Exibição dos Parâmetros escolhidos pelo usuário
        Console.WriteLine("Parâmetro\tValor");
        Console.WriteLine($"Cotação:\t{symbol}");
        Console.WriteLine($"Valor Máximo:\t{maxValue}");
        Console.WriteLine($"Valor Mínimo:\t{minValue}");
        Console.WriteLine(configFilePath);
        // Chamamos a classe StockQuoteFetcher para buscar o valor atual da Ação e substituir no Objeto RegularMarketPrice de StockQuote 
        StockQuoteFetcher fetcher = new StockQuoteFetcher();

        // Chamamos a Classe EmailNotifier para notificar o usuário
        EmailNotifier emailNotifier = new EmailNotifier(configFilePath);
        while(true)
        {
            try
            {   // Faz a busca da cotação de forma assíncrona (espera a conclusão do método FetchQuoteAsync)
                StockQuote quote = await fetcher.FetchQuoteAsync(symbol);
                Console.WriteLine($"Preço Atual:\t{quote.RegularMarketPrice}");

                // Primeira condição para mandar email
                if(quote.RegularMarketPrice > maxValue)
                {
                    Console.WriteLine("O preço de mercado está acima do valor máximo! Enviando email...");
                    emailNotifier.SendEmail(
                        "Ação acima do valor máximo",
                        $"A ação {symbol} está sendo negociada a {quote.RegularMarketPrice}, acima do valor máximo de {maxValue}. Considere vender."
                    );
                }
                // Segunda Condição para mandar email
                else if (quote.RegularMarketPrice < minValue)
                {
                    Console.WriteLine("O preço de mercado está abaixo do valor mínimo! Enviando email...");
                    emailNotifier.SendEmail(
                        "Ação abaixo do valor mínimo",
                        $"A ação {symbol} está sendo negociada a {quote.RegularMarketPrice}, abaixo do valor mínimo de {minValue}. Considere comprar."
                    );
                }
                // Se nenhuma das condições é atendida, seguimos em frente
            }
            catch (Exception ex)
            {
                // Mensagem de erro pro caso de ocorrer uma exceção
                Console.WriteLine($"Erro ao buscar cotação: {ex.Message}");
            }

            //Aguardar 5 minutos antes de enviar a próxima requisição
            Console.WriteLine("Aguardando 5 minutos antes da próxima verificação...");
            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }
}