using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.DTO.StaffDTO;
using MPTC_API.Models.StaffDTO;
using MPTC_API.Services.Authentication;

namespace MPTC_API.Services.Attendance
{
    public class LatenessResult
    {
        public float AvgLatenessDurationMinutes { get; set; }
    }
    public class StatsService
    {

        public static float GetPunctualityRate(int month, MptcContext _context)
        {
            Task<List<Models.DTO.LeaderboardDTO>>  leaderboards = AttendanceService.GetLeaderboardAsync(month, _context);

            if(leaderboards == null)
            {
                throw new Exception("An error occurred while fetching the leaderboard. Please try again later.");
            }

            float punctualityRate = 0;

            int totalWorkingDays = 0;
            int totalOnTimeCount = 0;
            //loop through LeaderboardDTO 
            foreach(Models.DTO.LeaderboardDTO leaderBoard in leaderboards.Result)
            {
                totalWorkingDays += leaderBoard.LatenessCount + leaderBoard.OnTimeCount + leaderBoard.AbsenceCount;
                totalOnTimeCount += leaderBoard.OnTimeCount;
               
            }
            punctualityRate = ((float) totalOnTimeCount / totalWorkingDays) * 100;

            Console.WriteLine($"Total Working Days: {totalWorkingDays} and Total On Time Count: {totalOnTimeCount}");
            Console.WriteLine($"Punctuality Rate: {(float) totalOnTimeCount / totalWorkingDays} ");
            return punctualityRate;

        }

        public static float  GetLatenessDurationAVG(int month, MptcContext _context)
        {
            int year = DateTime.Now.Year;
           // SQL query to get the average lateness duration
            var query = $@"
                SELECT 
                    AVG(AvgLatenessDurationMinutes) AS AvgLatenessDurationMinutes
                FROM 
                    public.""v_LatenessDurationAvg"" AS v
                WHERE v.year = {year};
            ";

            // Execute the query and retrieve the result as a dynamic object
              // Execute the query and retrieve the result
            var result = _context.Set<LatenessResult>()
                                .FromSqlRaw(query)
                                .AsEnumerable()
                                .FirstOrDefault(); // Gets the first (and only) result

            // Check if result is not null and print the average
           return (float) result.AvgLatenessDurationMinutes;

        }

        public static int GetTotalStaffNumber(MptcContext _context)
        {
            return _context.Staffs.Count();
        }
        
    }

}
