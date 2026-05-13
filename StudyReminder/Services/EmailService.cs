using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Microsoft.Extensions.Options;
using StudyReminder.Models;
using StudyReminder.Settings;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Task = System.Threading.Tasks.Task;

namespace StudyReminder.Services
{

    public class EmailService : IEmailService
    {
       
        private readonly Services.EmailTemplate _template;

        public EmailService(EmailTemplate template)
        {
            
            _template = template;
        }
     
        public async Task SendEmailAsync(string from, string to, string subject, string body)
        {
            
            brevo_csharp.Client.Configuration.Default.ApiKey["api-key"] = System.Environment.GetEnvironmentVariable("BREVO_API_KEY") ;
            var apiInstance = new TransactionalEmailsApi();
            var sendSmtpEmail = new SendSmtpEmail(
                sender: new SendSmtpEmailSender(email: from),
                to: new List<SendSmtpEmailTo> { new SendSmtpEmailTo(email: to) },
                subject: subject,
                htmlContent: body
                );

            try
            {
                await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            }
            catch(Exception ex)
            {
                Debug.Print("Exception when calling TransactionalEmailsApi.SendTransacEmail: " + ex.Message);
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
