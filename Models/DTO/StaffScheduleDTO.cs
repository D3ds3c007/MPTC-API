using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.StaffDTO
{
    public class StaffScheduleDTO
    {
        public int IdStaff { get; set; }
        public string Matricule { get; set; }
        public string StaffName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Date { get; set; }
    }
}
