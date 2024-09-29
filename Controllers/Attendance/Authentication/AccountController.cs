using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Web;
using System.Security.Claims;


namespace MPTC_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Member> _userManager;
        private readonly IEmailService _emailService;




        public MptcContext _context = new MptcContext();

        public AccountController(UserManager<Member> userManager,  IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;

        }

        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] MemberDTO MemberDTO)
        {
            Console.WriteLine(MemberDTO.Email);
            Member member = new Member();
            member.UserName = MemberDTO.Email;
            member.Email = MemberDTO.Email;
            member.NormalizedEmail = MemberDTO.Email.ToUpper();
            member.Password = BCrypt.Net.BCrypt.HashPassword(MemberDTO.Password);
            member.LastModified = DateTime.Now.ToUniversalTime();
            member.StaffId = 1;
            
            IdentityResult result = await _userManager.CreateAsync(member, MemberDTO.Password);
            return Ok(result);
        }
        [HttpPost("authentication")]
        public async Task<IActionResult> Authentication([FromBody] MemberDTO MemberDTO)
        {
            string token = null;
            Member member = null;
            try{
                member = _context.Members.Where(m => m.Email == MemberDTO.Email).FirstOrDefault();
                AccountService.authenticate(member, MemberDTO.Password);
                token = AccountService.GenerateJwtToken(member);
                return Ok(new {token = token});


            }catch(Exception e){
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("request-password-reset")]
        
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest passwordResetRequest)
        {
            var member = await _userManager.FindByEmailAsync(passwordResetRequest.Email);
            if(member == null){
                return NotFound("No user found with this email");
            }

            string resetToken = AccountService.GeneratePasswordResetToken(member);
            Console.WriteLine($"Generated Token: {resetToken}");


            string resetLink = "http://localhost:3000/accounts/reset-password?userId=" + member.Id + "&token=" + HttpUtility.UrlEncode(resetToken);
            

            //send email
            await _emailService.SendEmailAsync(member.Email, resetLink);

            Console.WriteLine(resetToken);
            return Ok(new {message = "Password reset link sent to your email if it exists"});
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest passwordResetRequest, [FromQuery] string userId, [FromQuery] string code)
        {
            var member = await _userManager.FindByIdAsync(userId);
            if(member == null){
                return NotFound("No user found with this email");
            }

            var token = WebUtility.UrlDecode(code);

            ClaimsPrincipal principal = AccountService.GetClaimsPrincipalFromToken(token);
            if(principal == null){
                return Unauthorized("Invalid token or expired token");
            }
            
            if(passwordResetRequest.Password != passwordResetRequest.ConfirmPassword){
                //
                return BadRequest("Passwords do not match. Please make sure both fields are identical.");
            }

            var newPassword = BCrypt.Net.BCrypt.HashPassword(passwordResetRequest.Password);
            member.Password = newPassword;
            await _userManager.UpdateAsync(member);

            return Ok(new {message = "Password reset successful"});
        }

        [HttpGet( "verify-reset-token")]
        public async Task<IActionResult> ValidateToken([FromQuery] string userId, [FromQuery] string token)
        {
            var member = await _userManager.FindByIdAsync(userId);
            if(member == null){
                return NotFound("No user found with this email");
            }

            token = WebUtility.UrlDecode(token);

            ClaimsPrincipal principal = AccountService.GetClaimsPrincipalFromToken(token);
            if(principal == null){
                return Unauthorized("Invalid token or expired token");
            }

            Console.WriteLine(principal.Identities.First().Claims.First().Value);
            return Ok(new {message = "Token is valid"});
        }



        

       

    }
}
