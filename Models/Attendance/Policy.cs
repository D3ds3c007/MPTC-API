using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance
{
    public class Policy
    {
        [Key]
        public int IdPolicy { get; set; }

        [Required(ErrorMessage = "Delay is required and cannot be empty")]
        public double Delay { get; set; }

        [Required(ErrorMessage = "Score is required and cannot be empty")]
        public double Score { get; set; }

        [Required(ErrorMessage = "Remark is required and cannot be empty")]
        public String Remark { get; set; }

        [Required(ErrorMessage = "PrivilegeId is required and cannot be empty")]
        public int PrivilegeId { get; set; }

        //navigation property
        public virtual Privilege Privilege { get; set; }
        public virtual ICollection<Sanction> Sanctions { get; set; }

    }

}
