using Domain.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BLL.Services
{
    public class SendGridEmailService : IEmailSender
    {
        public SendGridEmailSenderOptions OptionsValue;

        public SendGridEmailService(IOptions<SendGridEmailSenderOptions> options) => OptionsValue = options.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(OptionsValue.ApiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(OptionsValue.SenderEmail, OptionsValue.SenderName),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };

            msg.AddTo(new EmailAddress(email));
            await client.SendEmailAsync(msg);
        }
    }
}
