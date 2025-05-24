using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InfoSurge.Core.Interfaces;
using InfoSurge.Configuration;

namespace InfoSurge.Core.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient smtpClient;
        private readonly SmtpSettings smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            this.smtpSettings = smtpSettings.Value;
            this.smtpClient = new SmtpClient(smtpSettings.Value.Server)
            {
                Port = smtpSettings.Value.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpSettings.Value.SenderEmail, smtpSettings.Value.Password),
                EnableSsl = smtpSettings.Value.EnableSsl
            };
        }

        public async Task SendEmailAsync(string toUser, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings.SenderEmail, smtpSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toUser);
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                throw new InvalidOperationException($"Error sending email: {ex.Message}", ex);
            }
        }
    }
}
