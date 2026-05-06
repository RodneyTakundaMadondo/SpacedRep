using StudyReminder.Models;

namespace StudyReminder.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string from, string to, string subject, string body);
        Task SendRevisionReminderEmailAsync(string userEmail,List<StudyTopic> topics);
    }
}