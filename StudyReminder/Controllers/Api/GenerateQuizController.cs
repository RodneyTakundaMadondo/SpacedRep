using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyReminder.Models.Repositories;
using StudyReminder.Services;

namespace StudyReminder.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateQuizController : ControllerBase
    {
        private readonly IStudyFileRepository _studyFileRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGeminiService _geminiService;

        public GenerateQuizController(IStudyFileRepository studyFileRepository, IWebHostEnvironment webHostEnvironment, IGeminiService geminiService)
        {
            _studyFileRepository = studyFileRepository;
            _webHostEnvironment = webHostEnvironment;
            _geminiService = geminiService;
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetQuiz(int fileId)
        {
           
                var relativePath = await _studyFileRepository.GetFilePath(fileId);
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
                var quiz = await _geminiService.GenerateQuiz(fullPath);
                Console.WriteLine(quiz);
                return Ok(quiz);
            
           
        }
    }
}
