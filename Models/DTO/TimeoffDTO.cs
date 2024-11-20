using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance
{
    public class TimeOffDTO
    {
        public int IdTimeOff { get; set; }
        public String employeeName {get; set;}
        public String StaffMatricule { get; set; }
        public DateTime BeginTimeOff { get; set; }
        public DateTime EndTimeOff { get; set; }
    }
    
}
