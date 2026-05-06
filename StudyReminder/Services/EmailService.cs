using Microsoft.Extensions.Options;
using StudyReminder.Models;
using StudyReminder.Settings;
using System.Net;
using System.Net.Mail;

namespace StudyReminder.Services
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IOptions<SmtpSettings> _settings;
        private readonly Services.EmailTemplate _template;

        public EmailService(IConfiguration config, IOptions<SmtpSettings> settings, EmailTemplate template)
        {
            _config = config;
            _settings = settings;
            _template = template;
        }

        public async Task SendEmailAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);

            using (var emailClient = new SmtpClient(_settings.Value.Host, _settings.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(_settings.Value.User, _settings.Value.Password);
                await emailClient.SendMailAsync(message);
            }
        }

        public async Task SendRevisionReminderEmailAsync(string userEmail, List<StudyTopic> topics) //we are bringing in some study topics here
        {
            var topicsBlock = "";

            foreach (var topic in topics)
            {
                topicsBlock += $"<li>{topic.Title}</li>";
            }
            var topicContainer = $"<ul>{topicsBlock}</ul>";

            //get the html string from EmailTemplate
            var htmlBody = _template.GetReminderTemplate(topicContainer);
            var subject = $"Time to Review! You have {topics.Count} topic(s) due today";

            //after getting the string from. look at adding more key and values into the email template dictionary


            await this.SendEmailAsync("madondotakundaoriginal@gmail.com", userEmail, subject, htmlBody);
        }
    }
}
