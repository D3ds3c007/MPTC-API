using System.ComponentModel.DataAnnotations;

namespace MPTC_API.Models.Attendance.MemberDTO
{
    public class EmployeeFormDataDTO
    {
      
       public List<VenueDTO> Venues { get; set; }

       public List<PrivilegeDTO> Privileges { get; set; }

       public List<NationalityDTO> Nationalities { get; set; }


       
    }

}
