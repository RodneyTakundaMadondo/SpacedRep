namespace StudyReminder.Models
{
    public class Revision
    {
        public int Id { get; set; }
        public int RevisionNumber{get; set;}
        public int StudyTopicId { get; set; }
        public StudyTopic StudyTopic { get; set; } = default!;
        public DateTime ScheduledDate { get; set; }

    }
}
