using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class ProfSubject
    {
        [Key]
        public int IdProfSubject { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "SubjectId is required and cannot be empty")]
        public int SubjectId { get; set; }

        //navigation property
        [JsonIgnore]
        public virtual Staff Staff { get; set; }
        public virtual Subject Subject { get; set; }

    }

}
