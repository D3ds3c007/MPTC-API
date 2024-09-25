using System.ComponentModel.DataAnnotations;
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
    }

}
