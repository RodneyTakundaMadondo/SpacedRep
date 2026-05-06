using Microsoft.AspNetCore.Identity;
using StudyReminder.Models.Repositories;

namespace StudyReminder.Services
{
    public class RevisionReminderService
    {
        private readonly IStudyTopicRepository _studyTopicRepository;
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;

        public RevisionReminderService(IEmailService emailService, IStudyTopicRepository studyTopicRepository, UserManager<IdentityUser> userManager)
        {
            _emailService = emailService;
            _studyTopicRepository = studyTopicRepository;
            _userManager = userManager;
        }
        public async Task SendRevisionReminderAsync()
        {
            //get todays date
            var today  = DateTime.Today;

            //find revisions that are scheduled for today
            var todayRevisions = await _studyTopicRepository.GetRevisionsDueToday(today);
            if (todayRevisions == null) return;

            //group the studytpics by ownerid
            var userRevisions = todayRevisions.GroupBy(r => r.StudyTopic.OwnerId); // group by turns the result into something that works like a dictionary where the thing being grouped by becomes the key and value is the revisions in this case grouped by the topics ownerid

            foreach(var group in userRevisions)
            {
                var userId = group.Key??string.Empty;
                var user = await _userManager.FindByIdAsync(userId);
                var topics = group.Select(r => r.StudyTopic).ToList();

      

                await _emailService.SendRevisionReminderEmailAsync(user!.Email!,topics);
            }

        }
    }
}
