using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;

namespace MPTC_API.Models.Authentication
{
    public class Session
    {
        
        public static bool authenticate(Member member, String plainTextPassword)
        {
            if(member==null){
                throw new Exception("User not found");
            }
            //verify member password using bcrypt match
            if(BCrypt.Net.BCrypt.Verify(plainTextPassword, member.Password)){
                return true ;   
            }
            throw new Exception("Invalid credientials");
        }

        public static string GenerateJwtToken(Member member)
        {
           //create
           var claims = new[]
           {
                new Claim(ClaimTypes.NameIdentifier, member.IdMember.ToString()),
                new Claim(ClaimTypes.Email, member.Email),
                new Claim(ClaimTypes.Role, member.Staff.Privilege.PrivilegeName)
           };

           var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("K/RQJAY2USUtsgqE3bKzdIVX4DXX3jYB7M6z0RYyigQ="));
           var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }

}
