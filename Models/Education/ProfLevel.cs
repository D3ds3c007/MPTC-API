using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Education
{
    public class ProfLevel
    {
        [Key]
        public int IdProfLevel { get; set; }

        [Required(ErrorMessage = "ProfSubjectId is required and cannot be empty")]
        public int ProfSubjectId { get; set; }

        [Required(ErrorMessage = "LevelId is required and cannot be empty")]
        public int LevelId { get; set; }

        [Required(ErrorMessage = "PeriodId is required and cannot be empty")]
        public int PeriodId { get; set; }

        //navigation property
        public virtual ProfSubject ProfSubject { get; set; }
        public virtual Level Level { get; set; }
        public virtual Period Period { get; set; }

    }

}
