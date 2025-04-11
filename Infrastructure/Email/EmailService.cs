using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DotNetService.Infrastructure.Email
{
    public class EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        protected readonly EmailConfig _emailConfig = EmailConfig.ParseFromConfiguration(configuration);
        protected EventHandler<MessageSentEventArgs> MessageSentCallback = null;

        protected async Task<SmtpClient> CreateSmtpClient()
        {
            SmtpClient client = new()
            {
                ServerCertificateValidationCallback = delegate { return true; }
            };

            if (MessageSentCallback != null)
            {
                client.MessageSent += MessageSentCallback;
            }
            else
            {
                client.MessageSent += (sender, args) =>
                {
                    logger.LogInformation("Message sent: {MessageId}", args.Message.MessageId);
                };
            }

            if (_emailConfig.UseAuthentication)
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, _emailConfig.UseSsl);
                await client.AuthenticateAsync(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
            }
            else
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.None);
            }

            return client;
        }

        public async Task SendEmailAsync(Email email, EventHandler<MessageSentEventArgs> callback = null)
        {
            MessageSentCallback = callback;

            using SmtpClient client = await CreateSmtpClient();

            MimeMessage message = email.BuildMessage(_emailConfig);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendMultipleEmailAsync(IEnumerable<Email> emails, EventHandler<MessageSentEventArgs> callback = null)
        {
            MessageSentCallback = callback;

            using SmtpClient client = await CreateSmtpClient();

            foreach (var email in emails)
            {
                MimeMessage message = email.BuildMessage(_emailConfig);
                await client.SendAsync(message);
            }

            await client.DisconnectAsync(true);
        }
    }
}