using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class ResultNoteSection
    {
        [Key]
        public int IdResultNoteSection { get; set; }

        [Required(ErrorMessage = "ResultNoteId is required and cannot be empty")]
        public int ResultNoteId { get; set; }

        [Required(ErrorMessage = "SubjectSectionId is required and cannot be empty")]
        public int SubjectSectionId { get; set; }

        //navigation property
        public virtual ResultNote ResultNote { get; set; }
        public virtual SubjectSection SubjectSection { get; set; }

    }

}
