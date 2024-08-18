using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text;

public class EmailNotifier
{
    private string _destinationEmail = string.Empty;
    private string _smtpServer = string.Empty;
    private int _smtpPort = 0;
    private string _smtpUser = string.Empty;
    private string _smtpPassword = string.Empty;
    public EmailNotifier(string configFilePath)
    {
        LoadConfiguration(configFilePath);
    }

    private void LoadConfiguration(string configFilePath)
    {
        // Lê o arquivo de configuração
        string json = File.ReadAllText(configFilePath);

        // Analisa o JSON para obter as configurações
        var config = JsonSerializer.Deserialize<EmailConfiguration>(json);
        
        // Verifica se config é nulo
        if (config == null)
        {
            throw new InvalidOperationException("Falha ao carregar as configurações de email. Verifique se o arquivo de configuração está correto.");
        }

        // Atribui as configurações aos campos privados
        _destinationEmail = config.DestinationEmail ?? throw new InvalidOperationException("DestinationEmail não pode ser nulo.");
        _smtpServer = config.SmtpServer ?? throw new InvalidOperationException("SmtpServer não pode ser nulo.");
        _smtpPort = config.SmtpPort;
        _smtpUser = config.SmtpUser ?? throw new InvalidOperationException("SmtpUser não pode ser nulo.");
        _smtpPassword = DecodeBase64(config.SmtpPassword);
    }
        private string DecodeBase64(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public void SendEmail(string subject, string body)
    {
        using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage(_smtpUser, _destinationEmail, subject, body);
            smtpClient.Send(mailMessage);
        }
    }

    private class EmailConfiguration
    {
        public string DestinationEmail { get; set; } = string.Empty;
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 0;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
    }
}