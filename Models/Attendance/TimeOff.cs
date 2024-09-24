using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Attendance
{
    public class TimeOff
    {
        [Key]
        public int IdTimeOff { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "BeginTimeOff is required and cannot be empty")]
        public DateTime BeginTimeOff { get; set; }

        [Required(ErrorMessage = "EndTimeOff is required and cannot be empty")]
        public DateTime EndTimeOff { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }

    }

}
