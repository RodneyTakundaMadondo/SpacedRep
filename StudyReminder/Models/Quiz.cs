namespace StudyReminder.Models
{
    public class Quiz
    {
        public int id { get; set; }
        public string QuizData { get; set; } = string.Empty;
        public int StudyTopicId { get; set; }
        public StudyTopic? StudyTopic { get; set; }
        public int OwnerId { get; set; }



    }
}
