namespace MPTC_API.Models.Attendance.MemberDTO
{
    public class AttendanceRecordDTO
    {
       
       public string Matricule { get; set; }
       public TimeSpan ClockIn { get; set; }

       public TimeSpan? ClockOut { get; set; }

        public DateTime Date { get; set; }

        public string? Remarks { get; set; }
    
    }

}
