using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.DTO.StaffDTO
{
    public class StaffDTO
    {
      
        public int IdStaff   {get; set; }
        public string? Matricule {get; set;}
        public string? FullName {get; set;}

        public DateTime?  Birth { get; set; }
        public string? Gender {get; set;}
        public string? NationalId {get; set;}
        public string? PhoneNumber {get; set;}
        public string? Email {get; set;}

        public string Nationality {get; set;}
        public List<Schedule>? Schedule {get; set;}

        public string? Privilege {get; set;}
        public List<EmployeeImage>? Picture {get; set;}

    }

    
}
