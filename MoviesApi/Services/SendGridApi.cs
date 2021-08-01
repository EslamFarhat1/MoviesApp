using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MoviesApi.Services
{
    public  class SendGridApi
    {
        private IConfiguration Config { get; }
         public SendGridApi(IConfiguration config)
        {
            this.Config = config;

        }
        public  async Task<bool> SendMail(string userName, string Email, string plainTextContent,
                string htmlContent, string subject)
        {
            var apiKey = Config["ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("testExample@test.com", "test email");
            var to = new EmailAddress(Email, userName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return await Task.FromResult(true);
        }
    }
}