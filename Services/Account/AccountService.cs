using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;

namespace MPTC_API.Services.Authentication
{
    public class AccountService
    {
        public static readonly string  Secret = "K/RQJAY2USUtsgqE3bKzdIVX4DXX3jYB7M6z0RYyigQ=";

        public static async Task<IdentityResult> RegisterAsync(Staff staff, UserManager<Member> _userManager, string password)
        {
            Member member = new Member();


            member.UserName = staff.FirstName.Replace(" ", "") + staff.StaffName.Replace(" ", "");
            member.Email = staff.EmailAddress;
            member.NormalizedEmail = staff.EmailAddress.ToUpper();
            member.Password = BCrypt.Net.BCrypt.HashPassword(password);
            member.LastModified = DateTime.Now.ToUniversalTime();
            member.StaffId = staff.IdStaff;

            IdentityResult result = await _userManager.CreateAsync(member, password);

            //display the result

            result.Errors.ToList().ForEach(error => {
                Console.WriteLine(error.Description);
            });

            return result;
        }
        public static bool authenticate(Member member, String plainTextPassword)
        {
            if(member==null){
                throw new Exception("An error occurred. Please try again later.");
            }
            //verify member password using bcrypt match
            if(BCrypt.Net.BCrypt.Verify(plainTextPassword, member.Password)){
                return true ;   
            }
            throw new Exception("An error occurred. Please try again later.");
        }

        public static string GenerateJwtToken(Member member)
        {
            string url = "";
            switch(member.Staff.Privilege.PrivilegeName){
                case "administrator":
                    url = "http://localhost:3000/dashboard/administrator";
                    break;
                case "professor":
                    url = "http://localhost:3000/dashboard/professor";
                    break;
            }
           //create
           var claims = new[]
           {
                new Claim("id", member.IdMember.ToString()),
                new Claim("email", member.Email),
                new Claim("role", member.Staff.Privilege.PrivilegeName),
                new Claim("url", url)
           };

           var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
           var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public static string GeneratePasswordResetToken(Member member)
        {
             var tokenHandler = new JwtSecurityTokenHandler();
             var key = Encoding.ASCII.GetBytes(Secret);
        
             var tokenDescriptor = new SecurityTokenDescriptor
            {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim("UserID", member.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, member.Email)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(30), // Set token expiration
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secret);
            try{
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return principal;
            }
            catch{
                return null;
            }
        }
    }

}
