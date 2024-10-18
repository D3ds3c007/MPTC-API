using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Cms;

namespace MPTC_API.Models.Attendance
{
    public class Attendance 
    {
        [Key]
        public int IdAttendance { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Date is required and cannot be empty")]
        public DateTime  Date { get; set; }

        public TimeSpan?  ClockInTime  { get; set; }
        public TimeSpan?  ClockOutTime  { get; set; }
        public DateTime LastDetectedTime { get; set; }


        //navigation property
        public virtual Staff Staff { get; set; }
    }

}
