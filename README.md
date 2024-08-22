# Stock Quote Monitor

## Descrição

Este projeto é uma aplicação em C# desenvolvida para monitorar a cotação de uma ação específica e enviar notificações por email com base em valores de limite predefinidos. Quando a cotação da ação ultrapassa o valor máximo, o sistema aconselha a venda, e quando cai abaixo do valor mínimo, aconselha a compra.

## Funcionalidades

- **Monitoramento Contínuo**: Verifica a cotação da ação escolhida em intervalos de 5 minutos.
- **Notificação por Email**: Envia um email de notificação caso a cotação ultrapasse os limites estabelecidos.
- **Configuração Simples**: Configurações de email são gerenciadas através de um arquivo `emailsettings.json`.

## Pré-requisitos

- .NET Core SDK instalado no sistema.
- Um arquivo de configuração `emailsettings.json` no diretório principal do projeto.

## Estrutura do Projeto

- **`Program.cs`**: O ponto de entrada da aplicação, responsável por coordenar o monitoramento e o envio de notificações.
- **`StockQuote.cs`**: Classe que representa a cotação da ação.
- **`StockQuoteFetcher.cs`**: Classe responsável por buscar a cotação atual da ação de um serviço de API.
- **`EmailNotifier.cs`**: Classe responsável por enviar notificações por email.
- **`emailsettings.json`**: Arquivo de configuração contendo as informações de email (destinatário, servidor SMTP, etc.).

## Escolha da API
Uma das partes mais demoradas do projeto foi encontrar uma API que fornecesse cotações de ações da B3 em tempo hábil, com pouco atraso, e de forma gratuita. Considero que teria sido muito mais fácil obter essas cotações por meio de HTTP Parsing diretamente de sites de mercado financeiro, mas fiquei em dúvida sobre as implicações éticas dessa abordagem. Por isso, optei por uma solução mais clara e adequada, utilizando uma API.

Além disso, uma mudança no layout front-end dos sites consultados poderia comprometer os resultados do parser, tornando-o uma solução instável a longo prazo. Por outro lado a utilizando uma API, que é um produto desenvolvido e mantido por terceiros, não precisamos nos preocupar tanto com a manutenção contínua como no caso de um parser.

## API Utilizada

O projeto utiliza a [BRAPI](https://brapi.dev/) como fonte de dados para obter as cotações de ações. 

### Características Principais
- **Gratuita**: Oferece um plano gratuito que permite um número relativamente alto de requisições por mês, o que é interessante pro propósito do projeto já que fazemos uma checagem a cada 5min.
- **Dados em Tempo Real**: Fornece dados financeiros em tempo real (com um atraso de apenas 5 minutos pro plano gratuito).
- **Flexibilidade**: Suporta diversas faixas de tempo e intervalos, o que permite buscar dados históricos ou em tempo real com facilidade.

## Uso

### Argumentos Necessários

- **Símbolo da Ação**: O símbolo da ação a ser monitorada (por exemplo, "PETR4").
- **Valor Máximo**: O valor máximo aceitável para a cotação da ação, acima do qual uma notificação de venda será enviada.
- **Valor Mínimo**: O valor mínimo aceitável para a cotação da ação, abaixo do qual uma notificação de compra será enviada.

### Exemplo de Execução

#### Build inicial
Caso ainda não tenha feito a build inicial da aplicação, do diretório raiz do projeto rode o seguinte comando:
```bash
dotnet build
```

#### Exemplo de Execução do Binário
No diretório em que se encontra o binário (no meu caso `.../quoteTerminal/bin/Debug/netX.X/`) execute o comando:
```bash
./desafio PETR4 22.67 22.59
```
Além de PETR4, a aplicação permite monitorar outras diversas cotações.
##### O que posso usar em `args[0]`?
O símbolo das cotações disponíveis para monitoramento estão disponíveis no site: https://brapi.dev/quotes

### Estrutura do Arquivo `emailsettings.json`

Este arquivo deve conter as seguintes informações:

```json
{
    "DestinationEmail": "mail@example.com",
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUser": "user@example.com",
    "SmtpPassword": "c2VuaGE="
}
```

- **DestinationEmail**: Email para receber as notificações.
- **SmtpServer**: Servidor SMTP utilizado para enviar os emails.
- **SmtpPort**: Porta utilizada pelo servidor SMTP.
- **SmtpUser**: Usuário autenticado no servidor SMTP.
- **SmtpPassword**: Senha codificada em Base64.

#### Codificando a sua senha em base 64
Linux/MacOS: `echo -n "password" | base64`.

PowerShell: `[Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("password"))`.

## Email SMTP
Para testar o envio de emails utilizei o [MailerSend](https://app.mailersend.com). O MailerSend não exige etapas adicionais para se autenticar via SMTP (ao contrário de gmail e outlook), o que acabou tornando o processo mais simples.
