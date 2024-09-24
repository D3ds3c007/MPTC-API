using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class StudentLevel
    {
        [Key]
        public int IdStudentLevel { get; set; }

        [Required(ErrorMessage = "StudentId is required and cannot be empty")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "LevelId is required and cannot be empty")]
        public int LevelId { get; set; }

        [Required(ErrorMessage = "BeginDate is required and cannot be empty")]
        public DateTime BeginDate { get; set; }

        //navigation property
        public virtual Student Student { get; set; }
        public virtual Level Level { get; set; }

    }

}
