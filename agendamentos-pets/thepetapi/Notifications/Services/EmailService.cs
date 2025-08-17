using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Notifications.Models;
using Notifications.Services.Interfaces;

namespace Notifications.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
            {
                throw new ArgumentException("O endereço de e-mail do destinatário não pode ser nulo ou vazio.");
            }

            if (string.IsNullOrWhiteSpace(assunto))
            {
                throw new ArgumentException("O assunto não pode ser nulo ou vazio.", nameof(assunto));
            }

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                throw new ArgumentException("A mensagem não pode ser nula ou vazia.", nameof(mensagem));
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Pet's Agendamentos", _smtpSettings.From));
            emailMessage.To.Add(new MailboxAddress("", destinatario));
            emailMessage.Subject = assunto;
            emailMessage.Body = new TextPart("html") { Text = mensagem };

            using (var client = new SmtpClient())
            {
                // Ignorar a validação do certificado SSL
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                try
                {
                    // Conecte-se ao servidor SMTP
                    await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, false);
                    await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}