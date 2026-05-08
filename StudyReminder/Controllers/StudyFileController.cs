using Microsoft.AspNetCore.Mvc;
using StudyReminder.Models.Repositories;
using StudyReminder.Services;

namespace StudyReminder.Controllers
{
    public class StudyFileController : Controller
    {
        private readonly IStudyFileRepository _studyFileRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public StudyFileController(IStudyFileRepository studyFileRepository, ICloudinaryService cloudinaryService)
        {
            _studyFileRepository = studyFileRepository;
            _cloudinaryService = cloudinaryService;
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
                   
                    var file = await _studyFileRepository.GetStudyFileById(id.Value);
                    var cloudinaryId = file.PublicId;
                    //now we need to delete the file from cloudinary
                    await _cloudinaryService.DeleteUserNote(cloudinaryId);

                    // delete from our database 
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
