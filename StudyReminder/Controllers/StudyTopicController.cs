using Google.GenAI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using StudyReminder.Helpers;
using StudyReminder.Models;
using StudyReminder.Models.Repositories;
using StudyReminder.Services;
using StudyReminder.ViewModels;

namespace StudyReminder.Controllers
{
    [Authorize]
    public class StudyTopicController : Controller
    {
        private readonly IStudyTopicRepository _studyTopicRepository;
        private readonly IStudyFileRepository _studyFileRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGeminiService _geminiService;
        private readonly ICloudinaryService _cloudinaryService;
        public int fileSizeLimit = 5 * 1024 * 1024;


        public StudyTopicController(IStudyTopicRepository studyTopicRepository, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, IStudyFileRepository studyFileRepository, IGeminiService geminiService, ICloudinaryService cloudinaryService)
        {
            _studyTopicRepository = studyTopicRepository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _studyFileRepository = studyFileRepository;
            _geminiService = geminiService;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<IActionResult> TopicQuiz(int? fileId,int? topicId)
        {
            
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        [HttpPost]
        public async Task<IActionResult> Add(StudyTopicAddViewModel studyTopicAddViewModel, List<IFormFile> StudyFiles)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    
                 
                    var allowedMimeTypes= new Dictionary<string, string>
                    {
                        { "pdf", "application/pdf" },
                        { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                        { "txt", "text/plain" }
                    };
                    if(StudyFiles.Count > 1)
                    {
                        ModelState.AddModelError("StudyTopic.StudyFiles","Only one file can be uploaded per study topic.");
                        return View();
                    }

                    foreach (var file in StudyFiles) 
                    {
                        
                            if(file.Length > fileSizeLimit)
                            {
                                ModelState.AddModelError("StudyTopic.StudyFiles", "Attachment size limit exceeded. Maximum upload size is 5MB");
                                return View();
                            }
                            var fileExtension = DocumentHelper.GetExtension(file.FileName);
                            var mimetype = file.ContentType.ToLower();
                            if (!allowedMimeTypes.ContainsKey(fileExtension)) // check if the file extension of the file provided is allowed or not, this is important for quizzes, so block kids immediately from uploading those types of files
                            {
                                ModelState.AddModelError("StudyTopic.StudyFiles", "This file type is not supported yet.We only support (txt, pdf, docx)");
                                return View();
                            }

                            var pdfFileCheck = DocumentHelper.IsPdf(file); // checks if the file is a pdf
                            var wordFileCheck = DocumentHelper.IsWordDoc(file); // abstracted method that checks if a file is a word document
                            var textFileCheck = DocumentHelper.IsValidTextDoc(file); // abstracted method that checks if a file is a valid text document

                            if (!pdfFileCheck && !wordFileCheck && !textFileCheck)
                            {
                                ModelState.AddModelError("StudyTopic.StudyFiles", "This file type is not supported yet. We only support (txt, pdf, docx)");
                                return View();
                            }

                          
                            var uploadedFileDetails = await _cloudinaryService.SaveUserNotes(file);
                        studyTopicAddViewModel.StudyTopic.StudyFiles = new StudyFile
                        {
                            FileName = file.FileName,
                            FilePath = uploadedFileDetails.Item1,
                            PublicId = uploadedFileDetails.Item2,
                            FileSize = Math.Round(file.Length / (1024.0 * 1024.0), 2),
                            FileType = DocumentHelper.GetFileType(file.FileName)
                        };
                        

                    }

                    if (studyTopicAddViewModel.StudyTopic.DueDate.HasValue)
                    {
                        
                        int? totalDays = (studyTopicAddViewModel.StudyTopic.DueDate.Value.Date - studyTopicAddViewModel.StudyTopic.DateStarted.Value.Date).Days ;

                        double[] weights = { 0.10, 0.20, 0.35, 0.55, 0.75, 0.90 }; // these are intervals used to calculate the offsets from day 1 of studying, example revise after 10% of time has passed etc
                        
                        var revisionOffsets = new List<int>();

                        foreach (var w in weights)
                        {
                            var offset = (int)(totalDays * w);
                            revisionOffsets.Add(offset);
                        }

                        
                        var revisions = revisionOffsets.Select((offset,index)=> new Revision { ScheduledDate= studyTopicAddViewModel.StudyTopic.DateStarted.Value.AddDays(offset) , RevisionNumber = index+1}).ToList();

                        StudyTopic topic = new()
                        {
                            Title = studyTopicAddViewModel.StudyTopic.Title,
                            Description = studyTopicAddViewModel.StudyTopic.Description,
                            DateStarted = studyTopicAddViewModel.StudyTopic.DateStarted,
                            Revisions = revisions,
                            StudyFiles = studyTopicAddViewModel.StudyTopic.StudyFiles,
                            OwnerId = _userManager.GetUserId(User)

                        };

                        await _studyTopicRepository.AddStudyTopic(topic);
                        TempData["Success"] = "New Study Topic Successfully Added";
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                      
                        var revisionDays = new[] { 1, 2, 4, 7, 14, 30 };
                     
                        var revisions = revisionDays.Select((offset, index) => new Revision { ScheduledDate= studyTopicAddViewModel.StudyTopic.DateStarted.Value.AddDays(offset), RevisionNumber = index + 1 }).ToList();

                        StudyTopic topic = new()
                        {
                            Title = studyTopicAddViewModel.StudyTopic.Title,
                            Description = studyTopicAddViewModel.StudyTopic.Description,
                            DateStarted = studyTopicAddViewModel.StudyTopic.DateStarted,
                            Revisions = revisions,
                            StudyFiles = studyTopicAddViewModel.StudyTopic.StudyFiles,
                            OwnerId = _userManager.GetUserId(User)
                        };
                        await _studyTopicRepository.AddStudyTopic(topic);

                        TempData["Success"] = "New Study Topic Successfully Added";
                        return RedirectToAction("Index", "Home");
                    }
                }
           
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("StudyTopic", $"Error addding the study topic, please try again! Error: {ex.Message}");
            }
            return View();

        }

       

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    var topictoDelete = await _studyTopicRepository.GetStudyTopicByIdAsync(id.Value);

                    if (topictoDelete.StudyFiles is not null) {
                            await _cloudinaryService.DeleteUserNote(topictoDelete.StudyFiles.PublicId);     
                    }
                
                    await _studyTopicRepository.DeleteTopicAsync(id.Value);

                    TempData["Success"] = "Deleted Topic Successfully";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Invalid Id, unable to delete, please try again!";
                    return RedirectToAction("Index", "Home");
                }

            }catch(Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error deleting, please try again! Error: {ex.Message}";
               
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var selectedTopic = await _studyTopicRepository.GetStudyTopicByIdAsync(id);
            return View(selectedTopic);
        }
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        [HttpPost]
        public async Task<IActionResult> Edit(StudyTopic studyTopic, List<IFormFile> StudyFiles)
        {
            try
            {
                if(ModelState.IsValid)    
                {
                    if (StudyFiles.Count > 1)
                    {
                        ModelState.AddModelError("StudyFiles", "Only one file can be uploaded per study topic.");
                        return View(studyTopic);
                    }

                    var allowedMimeTypes = new Dictionary<string, string>
                    {
                        { "pdf", "application/pdf" },
                        { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                        { "txt", "text/plain" }
                    };

                    foreach (var file in StudyFiles)
                    {

                        if (file.Length > fileSizeLimit)
                        {
                            ModelState.AddModelError("StudyFiles", "Attachment size limit exceeded. Maximum upload size is 5MB");
                            return View(studyTopic);
                        }

                        var fileExtension = DocumentHelper.GetExtension(file.FileName);
                       
                            if (!allowedMimeTypes.ContainsKey(fileExtension)) // check if the file extension of the file provided is allowed or not, this is important for quizzes, so block kids immediately from uploading those types of files
                            {
                                ModelState.AddModelError("StudyFiles", "This file type is not supported yet. We only support (txt, pdf, docx)");
                                return View(studyTopic);
                            }

                            var pdfFileCheck = DocumentHelper.IsPdf(file); // checks if the file is a pdf
                            var wordFileCheck = DocumentHelper.IsWordDoc(file); // abstracted method that checks if a file is a word document
                            var textFileCheck = DocumentHelper.IsValidTextDoc(file); // abstracted method that checks if a file is a valid text document
                         
                            if(!pdfFileCheck && !wordFileCheck && !textFileCheck)
                            {
                                ModelState.AddModelError("StudyFiles", "This file type is not supported yet. We only support (txt, pdf, docx)");
                                return View(studyTopic);
                            }

                            var uploadedFileDetails = await _cloudinaryService.SaveUserNotes(file);

                        studyTopic.StudyFiles =new StudyFile
                        {
                            FileName = file.FileName,
                            FilePath = uploadedFileDetails.Item1,
                            PublicId = uploadedFileDetails.Item2,
                            FileSize = Math.Round(file.Length / (1024.0 * 1024.0), 2),
                            FileType = DocumentHelper.GetFileType(file.FileName)
                        };
                        
                    }

                    if (studyTopic.DueDate.HasValue)
                    {
                        var totalDays = (studyTopic.DueDate.Value.Date - studyTopic.DateStarted.Value.Date).Days;
                        double[] weights = { 0.10, 0.20, 0.35, 0.55, 0.75, 0.90 };

                        var revisionOffsets = new List<int>();
                        foreach(var w in weights)
                        {
                            var offset = (int)(totalDays * w);
                            revisionOffsets.Add(offset);
                        }

                        var revisions = revisionOffsets.Select((offset, index) => new Revision { ScheduledDate = studyTopic.DateStarted.Value.AddDays(offset), RevisionNumber = index + 1 }).ToList();
                        studyTopic.Revisions = revisions;
                      
                        await _studyTopicRepository.UpdateTopicAsync(studyTopic);
                        TempData["Success"] = "Updated  topic successfully";
                        return RedirectToAction("Index", "Home");
                    } 

                    else {
                        var revisionDays = new[] { 1, 2, 4, 7, 14, 30 };
                                         
                        var revisions = revisionDays.Select((offset, index) => new Revision { ScheduledDate = studyTopic.DateStarted.Value.AddDays(offset), RevisionNumber = index + 1 }).ToList();

                        studyTopic.Revisions = revisions;
                      

                        await _studyTopicRepository.UpdateTopicAsync(studyTopic);
                        TempData["Success"] = "Updated  topic successfully";
                        return RedirectToAction("Index", "Home");
                    }
                }

              

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("StudyTopic", $"Error updating the study topic, please try again! Error:{ex.Message}");
               
            }
            var selectedTopic = await _studyTopicRepository.GetStudyTopicByIdAsync(studyTopic.StudyTopicId);
            return View(selectedTopic);
        }

        public async Task<IActionResult> Details(int id)
        {
            var selectedStudyTopic = await _studyTopicRepository.GetStudyTopicByIdAsync(id);
            return View(selectedStudyTopic);

        }

       

       


    }
}
