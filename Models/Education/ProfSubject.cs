using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class ProfSubject
    {
        [Key]
        public int IdProfSubject { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "SubjectId is required and cannot be empty")]
        public int SubjectId { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<ResultNote> ResultNotes { get; set; }

    }

}
