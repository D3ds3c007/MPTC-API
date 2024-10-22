using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance.MemberDTO
{
    public class AttendanceDTO
    {
      
       public int AttendanceId { get; set; }
       public string Matricule { get; set; }
       public string StaffName { get; set; }
       public DateTime recordDate { get; set; }
       public TimeSpan? timeIn { get; set; }
       public TimeSpan? timeOut { get; set; } 
       public bool  isLate { get; set; }
       public string remark { get; set; }

       
    }

}
