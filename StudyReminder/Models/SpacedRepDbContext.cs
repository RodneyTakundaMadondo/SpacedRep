using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudyReminder.Models
{
    public class SpacedRepDbContext : IdentityDbContext
    {
        public SpacedRepDbContext(DbContextOptions<SpacedRepDbContext> options):base(options)
        {
            
        }

        public DbSet<StudyTopic> StudyTopics { get; set; }
        public DbSet<Revision> Revisions { get; set; }
        public DbSet<StudyFile> Files { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }

        
    }
}
