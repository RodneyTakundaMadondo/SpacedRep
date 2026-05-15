
using Microsoft.EntityFrameworkCore;

namespace StudyReminder.Models.Repositories
{
    public class StudyTopicRepository : IStudyTopicRepository
    {
        private readonly SpacedRepDbContext _context;

        public StudyTopicRepository(SpacedRepDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddStudyTopic(StudyTopic studyTopic)
        {
           _context.StudyTopics.Add(studyTopic);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteTopicAsync(int id)
        {
            var topicToBeDeleted = await _context.StudyTopics.FirstOrDefaultAsync(st => st.StudyTopicId == id);
            if (topicToBeDeleted != null)
            {
                _context.StudyTopics.Remove(topicToBeDeleted);
                return await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Error unable to locate the study topic to delete");
            }

        }

        public async Task<IEnumerable<StudyTopic>> GetAllStudyTopicsAsync(string userId)
        {
            return await _context.StudyTopics.Include(st=>st.Revisions!.OrderBy(rev=>rev.RevisionNumber)).Include(st=>st.StudyFiles).Where(st=>st.OwnerId == userId).OrderBy(st => st.StudyTopicId).ToListAsync();
        }

        public async Task<IEnumerable<Revision>> GetRevisionsDueToday(DateTime today)
        {
            var revisionDue = await _context.Revisions.Where(r=>r.ScheduledDate.Date == today.Date).Include(r=>r.StudyTopic).ToListAsync();
            return revisionDue;
        }

        public async Task<StudyTopic> GetStudyTopicByIdAsync(int id)
        {
            var selectedTopic =  await _context.StudyTopics.Include(st=>st.Revisions).Include(st=>st.StudyFiles).FirstOrDefaultAsync(st => st.StudyTopicId == id);
            if (selectedTopic != null)
            {
                return selectedTopic;
            }
            else
            {
                throw new Exception("Error finding the selected study topic");
            }
        }

        public async Task<int> UpdateTopicAsync(StudyTopic studyTopic)
        {
            var topicToUpdate = await _context.StudyTopics.Include(st=>st.Revisions).FirstOrDefaultAsync(st => st.StudyTopicId == studyTopic.StudyTopicId);
            if(topicToUpdate != null)
            {
                topicToUpdate.Title = studyTopic.Title;
                topicToUpdate.Description = studyTopic.Description; 
                topicToUpdate.DateStarted = studyTopic.DateStarted;
                topicToUpdate.Revisions.Clear();
                
                foreach(var rev in studyTopic.Revisions)
                {
                    topicToUpdate.Revisions.Add(rev);
                }
                topicToUpdate.StudyFiles = studyTopic.StudyFiles;
                _context.StudyTopics.Update(topicToUpdate);
                return await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Study topic to update not found");
            }
        }
    }
}
