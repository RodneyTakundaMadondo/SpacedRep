using Google.GenAI;
using Google.GenAI.Types;
using NuGet.Protocol;
using StudyReminder.Helpers;
using StudyReminder.Models;
using System.Security.Cryptography.Xml;

namespace StudyReminder.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly IConfiguration _config;

        public GeminiService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<string> GenerateQuizz(string quilJson)
        {
            var client = new Client(apiKey: System.Environment.GetEnvironmentVariable("GEMINI_API_KEY"));
            var extractedText = DocumentHelper.QuillToPlainText(quilJson);
            var devPrompt = "You will be given the extracted text from a plain text file containing study notes.\r\n\r\nYour task is to create a quiz based ONLY on the information in the document.\r\n\r\nSTRICT RULES:\r\n- Use ONLY information explicitly present in the text. Do NOT add outside knowledge.\r\n- Every question must be answerable directly from the text.\r\n- Ignore UI instructions, metadata, timestamps, and irrelevant sentences.\r\n- Generate exactly 10 questions.\r\n- Each question must have 4 answer options.\r\n- Exactly one correct answer per question.\r\n- Incorrect answers must be plausible and related to the topic, but still incorrect based on the text.\r\n- Do NOT repeat questions or concepts.\r\n\r\nOUTPUT FORMAT (STRICT JSON ONLY):\r\n[\r\n  {\r\n    \"question\": \"...\",\r\n    \"answers\": [\r\n      { \"text\": \"...\", \"correct\": true },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false }\r\n    ]\r\n  }\r\n]\r\n\r\nReturn ONLY the JSON array. No explanations, no markdown, no extra text.";
            var response = await client.Models.GenerateContentAsync(
                model: "gemini-3-flash-preview",
                contents: $"Here is the text: {extractedText} \n\n {devPrompt}",
                config: new GenerateContentConfig
                {
                    ResponseMimeType = "application/json"
                }
                );
            return response.Text ?? "[]";
        }
        public async Task<string> GenerateQuiz(StudyFile file,string cloudinaryPath)
        {
            var client = new Client(apiKey: System.Environment.GetEnvironmentVariable("GEMINI_API_KEY"));//define the client with the API key from secrets manager

           
            //we need to check what type of file it is first then evaluate whether we need to extract text or not. if its a pdf we can use a different pattern since gemini api loves pdfs and can extract text from them directly. if its a word document we need to extract the text first and then feed it to the gemini api. if its an unsupported file type we should return an error message.
            var fileType = DocumentHelper.GetFileType(file.FileName);
            if(fileType == "Microsoft Word Document")
            {
                string extractedText =await DocumentHelper.GetWordText(cloudinaryPath); // text that has been extracted from a word document using a helper class
                 string devPrompt = "You will be given the extracted text from a Word document containing study notes.\r\n\r\nYour task is to create a quiz based ONLY on the information in the document.\r\n\r\nSTRICT RULES:\r\n- Use ONLY information explicitly present in the text. Do NOT add outside knowledge.\r\n- Every question must be answerable directly from the text.\r\n- Ignore UI instructions, metadata, timestamps, and irrelevant sentences.\r\n- Generate exactly 10 questions.\r\n- Each question must have 4 answer options.\r\n- Exactly one correct answer per question.\r\n- Incorrect answers must be plausible and related to the topic, but still incorrect based on the text.\r\n- Do NOT repeat questions or concepts.\r\n\r\nOUTPUT FORMAT (STRICT JSON ONLY):\r\n[\r\n  {\r\n    \"question\": \"...\",\r\n    \"answers\": [\r\n      { \"text\": \"...\", \"correct\": true },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false }\r\n    ]\r\n  }\r\n]\r\n\r\nReturn ONLY the JSON array. No explanations, no markdown, no extra text.";
                var response = await client.Models.GenerateContentAsync(
                model: "gemini-3-flash-preview",
                contents: $"Here is the text: {extractedText} \n\n {devPrompt}",
                config: new GenerateContentConfig
                {
                    ResponseMimeType = "application/json",
                    ThinkingConfig = new ThinkingConfig
                    {
                        ThinkingLevel = ThinkingLevel.Minimal
                    }
                }
                );
                return response.Text ?? "[]";
            }
            else if (fileType == "PDF Document")
            {
               var pdfBytes = await DocumentHelper.GetPdfBytes(cloudinaryPath);
                var actualFile = await client.Files.UploadAsync(pdfBytes,null, new UploadFileConfig { MimeType = "application/pdf" });
                string devPrompt = "You will be given a PDF document containing study notes.\r\n\r\nYour task is to create a quiz based ONLY on the information in the document.\r\n\r\nSTRICT RULES:\r\n- Use ONLY information explicitly present in the text. Do NOT add outside knowledge.\r\n- Every question must be answerable directly from the text.\r\n- Ignore UI instructions, metadata, timestamps, and irrelevant sentences.\r\n- Generate exactly 10 questions.\r\n- Each question must have 4 answer options.\r\n- Exactly one correct answer per question.\r\n- Incorrect answers must be plausible and related to the topic, but still incorrect based on the text.\r\n- Do NOT repeat questions or concepts.\r\n\r\nOUTPUT FORMAT (STRICT JSON ONLY):\r\n[\r\n  {\r\n    \"question\": \"...\",\r\n    \"answers\": [\r\n      { \"text\": \"...\", \"correct\": true },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false }\r\n    ]\r\n  }\r\n]\r\n\r\nReturn ONLY the JSON array. No explanations, no markdown, no extra text.";
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents:new List<Content>
                    {
                        new Content
                        {
                            Parts = new List<Part>
                            {
                                new Part{FileData = new FileData {FileUri=actualFile.Uri,MimeType = actualFile.MimeType}},
                                new Part{Text = devPrompt}
                            }

                        }
                    },
                    config: new GenerateContentConfig
                    {
                        ResponseMimeType = "application/json",
                       
                    }
                    );
                return response.Text ?? "[]";
            }
            else if(fileType == "Plain Text File")
            {
                var extractedText = await DocumentHelper.GetTextFileText(cloudinaryPath);
                var devPrompt = "You will be given the extracted text from a plain text file containing study notes.\r\n\r\nYour task is to create a quiz based ONLY on the information in the document.\r\n\r\nSTRICT RULES:\r\n- Use ONLY information explicitly present in the text. Do NOT add outside knowledge.\r\n- Every question must be answerable directly from the text.\r\n- Ignore UI instructions, metadata, timestamps, and irrelevant sentences.\r\n- Generate exactly 10 questions.\r\n- Each question must have 4 answer options.\r\n- Exactly one correct answer per question.\r\n- Incorrect answers must be plausible and related to the topic, but still incorrect based on the text.\r\n- Do NOT repeat questions or concepts.\r\n\r\nOUTPUT FORMAT (STRICT JSON ONLY):\r\n[\r\n  {\r\n    \"question\": \"...\",\r\n    \"answers\": [\r\n      { \"text\": \"...\", \"correct\": true },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false },\r\n      { \"text\": \"...\", \"correct\": false }\r\n    ]\r\n  }\r\n]\r\n\r\nReturn ONLY the JSON array. No explanations, no markdown, no extra text.";
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-3-flash-preview",
                    contents: $"Here is the text: {extractedText} \n\n {devPrompt}",
                    config: new GenerateContentConfig
                    {
                        ResponseMimeType = "application/json"
                    }
                    );
                return response.Text ?? "[]";
            }
            else
            {
                return "Unsupported file type. Please upload a PDF or Word document.";
            }

          
            
        }
    }
}
