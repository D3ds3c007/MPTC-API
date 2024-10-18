using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class Exam
    {
        [Key]
        public int IdExam { get; set; }

        [Required(ErrorMessage = "Session is required and cannot be empty")]
        public int Session { get; set; }

        [Required(ErrorMessage = "SubjectId is required and cannot be empty")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "LevelId is required and cannot be empty")]
        public int LevelId { get; set; }

        [Required(ErrorMessage = "UripathAssetNote is required and cannot be empty")]
        public String UripathAssetNote { get; set; }

        [Required(ErrorMessage = "Uripath is required and cannot be empty")]
        public String Uripath { get; set; }

        [Required(ErrorMessage = "DateCreated is required and cannot be empty")]
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "StaffId is required and cannot be empty")]
        public int StaffId { get; set; }

        //navigation property
        public virtual Subject Subject { get; set; }
        public virtual Level Level { get; set; }
        [JsonIgnore]
        public virtual Staff Staff { get; set; }

    }

}
