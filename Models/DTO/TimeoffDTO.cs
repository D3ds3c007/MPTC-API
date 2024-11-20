using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance
{
    public class TimeOffDTO
    {
        public String StaffMatricule { get; set; }
        public DateTime BeginTimeOff { get; set; }
        public DateTime EndTimeOff { get; set; }
    }
    
}
