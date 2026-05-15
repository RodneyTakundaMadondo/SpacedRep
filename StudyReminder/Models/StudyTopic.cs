using System.ComponentModel.DataAnnotations;

namespace StudyReminder.Models
{
    public class StudyTopic
    {
        public int StudyTopicId { get; set; }
        
        [Required(ErrorMessage ="The Topic Title is required")]
        [StringLength(1000)]
        [Display(Name ="Topic Title")]
        public string Title { get; set; } = string.Empty;


        [Required(ErrorMessage ="Describe what you studied...")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage ="Date you started is required")]
        [Display(Name ="Date Started:")]
        [DisplayFormat(DataFormatString ="0:yyyy/MM/dd",ApplyFormatInEditMode =true)]
        [DataType(DataType.Date)]
        public DateTime? DateStarted { get; set; }
        [Display(Name ="Due Date:")]
      
        [DisplayFormat(DataFormatString = "0:yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        public ICollection<Revision>? Revisions { get; set; } = new List<Revision>();
        public StudyFile? StudyFiles { get; set; }
        public string? OwnerId { get; set; }

    }
}
