using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class ResultNote
    {
        [Key]
        public int IdResultNote { get; set; }

        [Required(ErrorMessage = "StudentId is required and cannot be empty")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "ExamId is required and cannot be empty")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "TotalPoint is required and cannot be empty")]
        public double TotalPoint { get; set; }

        [Required(ErrorMessage = "Percentage is required and cannot be empty")]
        public double Percentage { get; set; }

        [Required(ErrorMessage = "FinalScore is required and cannot be empty")]
        public double FinalScore { get; set; }

        [Required(ErrorMessage = "Accuracy is required and cannot be empty")]
        public double Accuracy { get; set; }

        [Required(ErrorMessage = "Status is required and cannot be empty")]
        public int Status { get; set; }

        [Required(ErrorMessage = "ProfSubjectId is required and cannot be empty")]
        public int ProfSubjectId { get; set; }

        [Required(ErrorMessage = "Date is required and cannot be empty")]
        public DateTime Date { get; set; }

        //navigation property
        public virtual Student Student { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual ProfSubject ProfSubject { get; set; }
        public virtual ICollection<ResultNoteSection> ResultNoteSections { get; set; }

    }

}
