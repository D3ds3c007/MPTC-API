using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance
{
    public class Schedule
    {
        [Key]
        public int IdSchedule { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "DayOfWeek is required and cannot be empty")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Begin is required and cannot be empty")]
        public TimeSpan  Begin { get; set; }

        [Required(ErrorMessage = "End is required and cannot be empty")]
        public  TimeSpan  End { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }

    }

}
