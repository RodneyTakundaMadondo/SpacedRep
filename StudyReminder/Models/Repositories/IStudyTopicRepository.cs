namespace StudyReminder.Models.Repositories
{
    public interface IStudyTopicRepository
    {
        Task<IEnumerable<StudyTopic>> GetAllStudyTopicsAsync(string userId);
        Task<StudyTopic> GetStudyTopicByIdAsync(int id);

        Task<int> AddStudyTopic(StudyTopic studyTopic);
        Task<int> DeleteTopicAsync(int id);
        Task<int> UpdateTopicAsync(StudyTopic studyTopic);

        Task<IEnumerable<Revision>> GetRevisionsDueToday(DateTime today);
    }
}
