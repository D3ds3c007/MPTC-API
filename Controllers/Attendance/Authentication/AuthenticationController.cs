using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using BCrypt.Net;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;
using MPTC_API.Services.Authentication;


namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {

        public MptcContext _context = new MptcContext();

        [HttpPost(Name = "authentication")]
        public async Task<IActionResult> Authentication([FromBody] MemberDTO MemberDTO)
        {
            string token = null;
            Member member = null;
            try{
                member = _context.Members.Where(m => m.Email == MemberDTO.Email).FirstOrDefault();
                Session.authenticate(member, MemberDTO.Password);
                token = Session.GenerateJwtToken(member);
                return Ok(MemberDTO);


            }catch(Exception e){
                return Unauthorized(e.Message);
            }
        }

       

    }
}
