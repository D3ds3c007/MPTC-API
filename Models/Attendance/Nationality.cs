using System.ComponentModel.DataAnnotations;
using MPTC_API.Models.Education;

namespace MPTC_API.Models.Attendance
{
    public class Nationality
    {
        [Key]
        public int IdNationality { get; set; }

        [Required(ErrorMessage = "NationalityName is required and cannot be empty")]
        public String NationalityName { get; set; }

        //navigation property
        public virtual ICollection<Staff> Staffs { get; set; }
        public virtual ICollection<Student> Students { get; set; }

    }

}
