using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class TempResultSection
    {
        [Key]
        public int IdTempResultSection { get; set; }

        [Required(ErrorMessage = "TempResultId is required and cannot be empty")]
        public int TempResultId { get; set; }

        [Required(ErrorMessage = "SubjectSectionId is required and cannot be empty")]
        public int SubjectSectionId { get; set; }

        [Required(ErrorMessage = "Score is required and cannot be empty")]
        public double Score { get; set; }

        //navigation property
        public virtual TempResult TempResult { get; set; }
        public virtual SubjectSection SubjectSection { get; set; }

    }

}
