using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.BigData;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.Attendance
{
    public class Staff
    {
        [Key]
        public int IdStaff { get; set; }

        public String? Matricule { get; set; }

        [Required(ErrorMessage = "StaffName is required and cannot be empty")]
        public String StaffName { get; set; }

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

        [Required(ErrorMessage = "IDCardNumber is required and cannot be empty")]
        public String IDCardNumber { get; set; }

        [Required(ErrorMessage = "HomeAddress is required and cannot be empty")]
        public String HomeAddress { get; set; }

        [Required(ErrorMessage = "MaritalStatus is required and cannot be empty")]
        public String MaritalStatus { get; set; }

        [Required(ErrorMessage = "NationalityId is required and cannot be empty")]
        public int NationalityId { get; set; }

        [Required(ErrorMessage = "PrivilegeId is required and cannot be empty")]
        public int PrivilegeId { get; set; }

        //navigation property
        public virtual Venue Venue { get; set; }
        public virtual Privilege Privilege { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual Member Member { get; set; }
        public virtual ICollection<Sanction> Sanctions { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<TimeOff> TimeOffs { get; set; }
        public virtual ICollection<ProfSubject> ProfSubjects { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<ResultNote> ResultNotes { get; set; }

    }

}
