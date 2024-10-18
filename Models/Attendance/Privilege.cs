using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MPTC_API.Models.Attendance
{
    public class Privilege
    {
        [Key]
        public int IdPrivilege { get; set; }

        [Required(ErrorMessage = "PrivilegeName is required and cannot be empty")]
        public String PrivilegeName { get; set; }

        //navigation property
        [JsonIgnore]
        public virtual ICollection<Staff> Staffs { get; set; }
        public virtual ICollection<Policy> Policies { get; set; }

    }

}
