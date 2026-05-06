using Microsoft.AspNetCore.Mvc;
using StudyReminder.Models.Repositories;

namespace StudyReminder.Controllers
{
    public class StudyFileController : Controller
    {
        private readonly IStudyFileRepository _studyFileRepository;

        public StudyFileController(IStudyFileRepository studyFileRepository)
        {
            _studyFileRepository = studyFileRepository;
        }

        [HttpPost]
        public async Task<IActionResult>DeleteDoc(int? id)
        {
            try
            {
                if(id == null)
                {
                    ViewData["ErrorMessage"] = "Invalid Id!, please try again!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await _studyFileRepository.DeleteFile(id.Value);
                    ViewData["FileDeleted"] = "File deleted successfully";
                    return RedirectToAction("Index","Home");
                }
            }catch(Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error Deleting document, please try again! Error: {ex.Message}";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
