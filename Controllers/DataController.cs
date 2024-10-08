using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Models.Attendance.MemberDTO;



namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;




        public MptcContext _context = new MptcContext();

        public DataController(UserManager<Member> userManager)
        {
            _userManager = userManager;

        }

        [HttpGet("form-data")]
        public async Task<IActionResult> GetVenues()
        {
            EmployeeFormDataDTO employeeFormDataDTO = DataService.GetEmployeeFormDataDTO(_context);
            return Ok(employeeFormDataDTO);
        }




        

       

    }
}
