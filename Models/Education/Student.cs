using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Attendance;

namespace MPTC_API.Models.Education
{
    public class Student
    {
        [Key]
        public int IdStudent { get; set; }

        [Required(ErrorMessage = "Matricule is required and cannot be empty")]
        public String Matricule { get; set; }

        [Required(ErrorMessage = "StudentName is required and cannot be empty")]
        public String StudentName { get; set; }

        [Required(ErrorMessage = "FirstName is required and cannot be empty")]
        public String FirstName { get; set; }

        [Required(ErrorMessage = "Birth is required and cannot be empty")]
        public DateTime Birth { get; set; }

        [Required(ErrorMessage = "VenueId is required and cannot be empty")]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Gender is required and cannot be empty")]
        public String Gender { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required and cannot be empty")]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "EmailAddress is required and cannot be empty")]
        public String EmailAddress { get; set; }

        [Required(ErrorMessage = "HomeAddress is required and cannot be empty")]
        public String HomeAddress { get; set; }

        [Required(ErrorMessage = "NationalityId is required and cannot be empty")]
        public int NationalityId { get; set; }

        //navigation property
        public virtual Venue Venue { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual ICollection<ResultNote> ResultNotes { get; set; }
        public virtual ICollection<StudentLevel> StudentLevels { get; set; }
        public virtual ICollection<TempResult> TempResults { get; set; }

    }

}
