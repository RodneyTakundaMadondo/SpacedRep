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
        private readonly IStudyTopicRepository _studyTopicRepository;

        public GenerateQuizController(IStudyFileRepository studyFileRepository, IWebHostEnvironment webHostEnvironment, IGeminiService geminiService, IStudyTopicRepository studyTopicRepository)
        {
            _studyFileRepository = studyFileRepository;
            _webHostEnvironment = webHostEnvironment;
            _geminiService = geminiService;
            _studyTopicRepository = studyTopicRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuiz(int? fileId,int? topicId)
        {
           if(fileId!= null)
            {
                var file = await _studyFileRepository.GetStudyFileById(fileId.Value);
                var cloudPath = file.FilePath;
                var quiz = await _geminiService.GenerateQuiz(file, cloudPath);

                return Ok(quiz);
            }

           if(topicId!= null)
            {
                var studyTopic = await _studyTopicRepository.GetStudyTopicByIdAsync(topicId.Value);
                var userTextNote = studyTopic.Description;
                //send the text to a helper method to get processed 
                var quiz = await _geminiService.GenerateQuizz(userTextNote);
                //send the processed text to 
                return Ok(quiz);
            }

            return Ok();
                
            
           
        }
    }
}
