using Microsoft.AspNetCore.Mvc;
using MPTC_API.Data;
using BCrypt.Net;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;
using MPTC_API.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Web;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;


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
            member.StaffId = 3;
            member.SecurityStamp = Guid.NewGuid().ToString();
            
            IdentityResult result = await _userManager.CreateAsync(member, MemberDTO.Password);
            return Ok(result);
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
            var member = await _userManager.FindByEmailAsync(passwordResetRequest.Email);
            if(member == null){
                return NotFound("No user found with this email");
            }

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(member);

                Console.WriteLine($"Generated Token: {resetToken}");


            // IdentityResult isValid = await _userManager.ResetPasswordAsync(member, resetToken, "Raitra123##@@Vip");

            // bool isValid = await _userManager.VerifyUserTokenAsync(member, _userManager.Options.Tokens.PasswordResetTokenProvider, UserManager<Member>.ResetPasswordTokenPurpose, resetToken);
            
            //generate reset link

            string resetLink = "http://localhost:3000/api/v1/authentication/reset-password?email=" + passwordResetRequest.Email + "&token=" + resetToken;
            
            resetToken =  WebUtility.UrlEncode(resetToken);
            //send email
            // await _emailService.SendEmailAsync(member.Email, resetLink);

            // Set a cookie with the reset token
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevent JavaScript access to this cookie
                Secure = true,   // Ensure it's sent over HTTPS only (set to true in production)
                SameSite = SameSiteMode.Strict, // Ensures the cookie is sent only with same-site requests
                Expires = DateTime.UtcNow.AddMinutes(30) // Cookie expiry time, adjust as necessary
            };
    
            // Store the reset token in the cookie
            HttpContext.Response.Cookies.Append("PasswordResetToken", resetToken, cookieOptions);

            return Ok(new {token = resetToken});
        }


        [HttpGet( "reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string token)
        {
            var member = await _userManager.FindByEmailAsync(email);
            if(member == null){
                return NotFound("No user found with this email");
            }

            

           //change the password
           IdentityResult result =  await _userManager.ResetPasswordAsync(member, token, "Raitra123##");

           if(result.Succeeded)
           {
                return Ok(result.Succeeded);
           }else{
                return BadRequest(result.Errors);
           }

        
        }

        

       

    }
}
