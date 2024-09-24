using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class Level
    {
        [Key]
        public int IdLevel { get; set; }

        [Required(ErrorMessage = "LevelName is required and cannot be empty")]
        public String LevelName { get; set; }

        //navigation property
        public virtual ICollection<SubjectSection> SubjectSections { get; set; }
        public virtual ICollection<StudentLevel> StudentLevels { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }

    }

}
