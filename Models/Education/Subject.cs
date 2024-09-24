using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class Subject
    {
        [Key]
        public int IdSubject { get; set; }

        [Required(ErrorMessage = "SubjectName is required and cannot be empty")]
        public String SubjectName { get; set; }

        //navigation property
        public virtual ICollection<SubjectSection> SubjectSections { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<ProfSubject> ProfSubjects { get; set; }
    }

}
