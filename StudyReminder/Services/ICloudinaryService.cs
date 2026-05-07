namespace StudyReminder.Services
{
    public interface ICloudinaryService
    {
        Task<(string, string)> SaveUserNotes(IFormFile file);
        Task DeleteUserNote(string publicId);
    }
}