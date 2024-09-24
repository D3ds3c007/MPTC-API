using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class TempResult
    {
        [Key]
        public int IdTempResult { get; set; }

        [Required(ErrorMessage = "StudentId is required and cannot be empty")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "ExamId is required and cannot be empty")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "FinalScore is required and cannot be empty")]
        public double FinalScore { get; set; }

        [Required(ErrorMessage = "Accuracy is required and cannot be empty")]
        public double Accuracy { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "DateCreated is required and cannot be empty")]
        public DateTime DateCreated { get; set; }

        //navigation property
        public virtual Student Student { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ICollection<TempResultSection> TempResultSections {get; set;}
    }

}
