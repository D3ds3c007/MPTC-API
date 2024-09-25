using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;

namespace MPTC_API.Services.Authentication
{
    public class Session
    {
        
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
