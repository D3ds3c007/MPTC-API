using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance.MemberDTO
{
    public class PasswordResetRequest
    {
          public string Email { get; set; }

          public string Password { get; set; }

          public string ConfirmPassword { get; set; }
    }

}
