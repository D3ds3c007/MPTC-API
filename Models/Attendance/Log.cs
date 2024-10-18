using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MPTC_API.Models.Attendance
{
    public class Log
    {
        [Key]
        public int IdLogs { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "EventType is required and cannot be empty")]
        public string EventType { get; set; }

        [Required(ErrorMessage = "EventTime is required and cannot be empty")]
        public DateTime EventTime { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }

    }

}
