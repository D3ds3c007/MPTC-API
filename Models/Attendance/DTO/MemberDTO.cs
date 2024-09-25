using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance.MemberDTO
{
    public class MemberDTO
    {
      
        public String Email { get; set; }

        [Required(ErrorMessage = "Password is required and cannot be empty")]
        public String Password { get; set; }


       
    }

}
