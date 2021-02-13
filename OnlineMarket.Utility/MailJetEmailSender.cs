using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace OnlineMarket.Utility
{
    public class MailJetEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public MailJetSettings _mailJetSettings { get; set; }

        public MailJetEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey)
            {
                Version = ApiVersion.V3_1,              
            };

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.Messages, new JArray {
                new JObject {
                {
                    "From",
                    new JObject {
                        {"Email", "nexus251089@protonmail.com"},
                        {"Name", "nexus251089"}
                }},
                {"To",
                new JArray {
                new JObject {
                    {
                    "Email",
                    email
                },{
                    "Name",
                    "aiomarket.kz" }}}},
                {
                    "Subject",
                    subject },
                {
                    "HTMLPart",
                    body },
                }});
            await client.PostAsync(request);
        }
    }
}
