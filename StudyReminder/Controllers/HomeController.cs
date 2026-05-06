using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyReminder.Models.Repositories;
using StudyReminder.ViewModels;

namespace StudyReminder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudyTopicRepository _studyTopicRepository;
        private readonly UserManager<IdentityUser> _userManager;



        public HomeController( IStudyTopicRepository studyTopicRepository, UserManager<IdentityUser> userManager)
        {
            
            _studyTopicRepository = studyTopicRepository;
            _userManager = userManager;
        }

        public async  Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            HomeViewModel model = new() { StudyTopics = await _studyTopicRepository.GetAllStudyTopicsAsync(userId) };
            return View(model);
        }


        
    }
}
