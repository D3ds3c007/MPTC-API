using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance
{
    public class EmployeeImage
    {
        public int IdStaff { get; set; }
        public string? Base64Image { get; set; }
        public float[]? Descriptor { get; set; }

     

    }

}
