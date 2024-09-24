using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance
{
    public class Log
    {
        [Key]
        public int IdLogs { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "DateTime is required and cannot be empty")]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "IsDateTimeIn is required and cannot be empty")]
        public DateTime IsDateTimeIn { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }

    }

}
