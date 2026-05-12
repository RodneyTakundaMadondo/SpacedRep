using StudyReminder.Models;

namespace StudyReminder.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateQuiz(StudyFile file, string cloudinaryPath);
        Task<string> GenerateQuizz(string quilJson);
    }
}