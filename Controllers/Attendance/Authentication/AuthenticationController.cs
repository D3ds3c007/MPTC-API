using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using BCrypt.Net;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;


namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;


        public MptcContext _context = new MptcContext();

        public AuthenticationController(UserManager<Member> userManager,  IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;

        }

        [HttpPost(Name = "authentication")]
        public async Task<IActionResult> Authentication([FromBody] MemberDTO MemberDTO)
        {
            string token = null;
            Member member = null;
            try{
                member = _context.Members.Where(m => m.Email == MemberDTO.Email).FirstOrDefault();
                Session.authenticate(member, MemberDTO.Password);
                token = Session.GenerateJwtToken(member);
                return Ok(new {token = token});


            }catch(Exception e){
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest passwordResetRequest)
        {
            Member member = _context.Members.Where(m => m.Email == passwordResetRequest.Email).FirstOrDefault();
            if(member == null){
                return NotFound("No user found with this email");
            }
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(member);

            //generate reset link
            string resetLink = "http://localhost:5000/api/v1/authentication/reset-password?email=" + passwordResetRequest.Email + "&token=" + resetToken;

            //send email
            await _emailService.SendEmailAsync(member.Email, resetLink);

            Console.WriteLine(resetLink + "hehe");
            return Ok(new {resetUrl = resetLink});
        }

       

    }
}
