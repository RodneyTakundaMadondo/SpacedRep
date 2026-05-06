namespace StudyReminder.Models
{
    public class StudyFile
    {
        public int Id { get; set; } 
        public string FileName { get; set; } = string.Empty; 
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get;  set; } 
        public double FileSize { get; set; }
        
        public int StudyTopicId { get; set; }

        public StudyTopic StudyTopic { get; set; } = default!;
        
    }
}
