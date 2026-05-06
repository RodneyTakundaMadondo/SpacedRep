
using Microsoft.EntityFrameworkCore;

namespace StudyReminder.Models.Repositories
{
    public class StudyFileRepository : IStudyFileRepository
    {
        private readonly SpacedRepDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudyFileRepository(SpacedRepDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<int> DeleteFile(int? id)
        {
            var fileToDelete = await _context.Files.FirstOrDefaultAsync(f=>f.Id == id.Value);
            if(fileToDelete != null)
            {
                var relativePath = fileToDelete.FilePath.TrimStart('/');
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath,relativePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                _context.Files.Remove(fileToDelete);
               return await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("File to delete not found");
            }
            
        }
        public async Task<string> GetFilePath(int? id)
        {
            if(id == null)
            {
                throw new Exception("File id is null");
            }
            var file = await _context.Files.FirstOrDefaultAsync(f=>f.Id == id.Value);
            if (file != null)
            {
                return file.FilePath.TrimStart('/');
            }
            else
            {
                throw new Exception("File not found");
            }

        }
    }
}
