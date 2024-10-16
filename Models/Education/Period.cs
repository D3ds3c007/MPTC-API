using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class Period
    {
        [Key]
        public int IdPeriod { get; set; }

        [Required(ErrorMessage = "BeginDate is required and cannot be empty")]
        public DateTime BeginDate { get; set; }

        [Required(ErrorMessage = "EndDate is required and cannot be empty")]
        public DateTime EndDate { get; set; }

        //navigation property
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<StudentLevel> StudentLevels { get; set; }
        public virtual ICollection<ProfLevel> ProfLevels { get; set; }

    }

}
