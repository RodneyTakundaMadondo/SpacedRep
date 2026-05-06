namespace StudyReminder.Services
{
    public class EmailTemplate
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailTemplate(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetTemplate(string confirmationLink, string exactFileName, string wordToBeReplaced)
        {

            //get the path to the html file
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", $"{exactFileName}");

            //read all the text in the path
            var emailTemplate = File.ReadAllText(filePath);
            //create a dictionary to store key and value pairs for what we want to replace in the html document
            var replacementDictionary = new Dictionary<string, string>
            {
                { $"{{{{{wordToBeReplaced}}}}}",confirmationLink},
            };

            //loop through the dictionary and replace each item
            foreach(var item in replacementDictionary)
            {
                emailTemplate = emailTemplate.Replace(item.Key, item.Value);
            }
            return emailTemplate;
        }
        public string GetReminderTemplate(string topicBlock)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "reviewTopics.html");

            var emailTemplate = File.ReadAllText(filePath);
            var replacementDictionary = new Dictionary<string, string>
            {
                {"{{topicsBlock}}",topicBlock}
            };

            foreach(var item in replacementDictionary)
            {
                emailTemplate = emailTemplate.Replace(item.Key, item.Value);
            }
            return emailTemplate; 
        }
    }
}
