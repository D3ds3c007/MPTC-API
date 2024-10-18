using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.Attendance
{
    public class Venue
    {
        [Key]
        public int IdVenue { get; set; }

        [Required(ErrorMessage = "VenueName is required and cannot be empty")]
        public String VenueName { get; set; }

        //navigation property
        [JsonIgnore]
        public virtual ICollection<Staff> Staffs { get; set; }
        public virtual ICollection<Student> Students { get; set; }

    }

}
