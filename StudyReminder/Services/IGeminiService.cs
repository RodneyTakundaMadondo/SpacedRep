namespace StudyReminder.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateQuiz(string file);
    }
}