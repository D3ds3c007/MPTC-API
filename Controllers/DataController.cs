using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;



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





        

       

    }
}
