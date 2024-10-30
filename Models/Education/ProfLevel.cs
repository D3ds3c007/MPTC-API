using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class ProfLevel
    {
        [Key]
        public int IdProfLevel { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "LevelId is required and cannot be empty")]
        public int LevelId { get; set; }

        [Required(ErrorMessage = "PeriodId is required and cannot be empty")]
        public int PeriodId { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }
        public virtual Level Level { get; set; }
        public virtual Period Period { get; set; }

    }

}
