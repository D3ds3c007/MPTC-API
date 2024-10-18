using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MPTC_API.Models.Attendance
{
    public class Sanction
    {
        [Key]
        public int IdSanction { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "DateSanction is required and cannot be empty")]
        public DateTime DateSanction { get; set; }

        [Required(ErrorMessage = "PolicyId is required and cannot be empty")]
        public int PolicyId { get; set; }

        //navigation property
        [JsonIgnore]
        public virtual Staff Staff { get; set; }
        public virtual Policy Policy { get; set; }
    }

}
