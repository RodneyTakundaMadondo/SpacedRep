namespace StudyReminder.Models.Repositories
{
    public interface IStudyFileRepository
    {
        Task<int> DeleteFile(int? id);
        Task<string> GetFilePath(int? id);
        Task<StudyFile> GetStudyFileById(int id);
    }
}
