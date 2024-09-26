using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MPTC_API.Models.Attendance
{
    public class Member : IdentityUser
    {
        [Key]
        public int IdMember { get; set; }

        [Required(ErrorMessage = "Email is required and cannot be empty")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Password is required and cannot be empty")]
        public String Password { get; set; }

        [Required(ErrorMessage = "LastModified is required and cannot be empty")]
        public DateTime LastModified { get; set; }

         // Add SecurityStamp property manually
        public string SecurityStamp { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        //navigation property
        public virtual Staff Staff { get; set; }
    }

}
