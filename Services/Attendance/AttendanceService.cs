using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MPTC_API.Data;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.StaffDTO;
using MPTC_API.Services.Authentication;

namespace MPTC_API.Services.Attendance
{
    public class AttendanceService
    {
        public static void LogAttendance(int StaffId, bool isClockIn, MptcContext context)
        {
            Console.WriteLine("Loging attendance");
            var log = new Log
            {
                StaffId = StaffId,
                EventType = isClockIn? "ClockIn": "ClockOut",
                EventTime = DateTime.UtcNow
            };
            context.Logss.Add(log);
            context.SaveChanges();
        }

        public static async Task UpdateAttendanceAsync(int StaffId, bool isClockIn, MptcContext context)
        {
            Console.WriteLine("Insert Attendance");
            DateTime? lastDetectedTime = context.Attendances
                                        .Where(a => a.StaffId == StaffId)
                                        .OrderByDescending(a => a.Date)
                                        .Select(a => (DateTime?)a.LastDetectedTime)
                                        .FirstOrDefault();


            DateTime currentTime = DateTime.UtcNow;

            if(lastDetectedTime == null || (currentTime-lastDetectedTime.Value).TotalMinutes >= 10)
            {
                Console.WriteLine($"lastedetecteTime : {lastDetectedTime}");
                Console.WriteLine("Tafiditra attendance");
                MPTC_API.Models.Attendance.Attendance attendance = new MPTC_API.Models.Attendance.Attendance
                {
                    StaffId = StaffId,
                    Date = DateTime.UtcNow,
                    ClockInTime = isClockIn ? DateTime.UtcNow.AddHours(3).TimeOfDay : (TimeSpan?)null,
                    ClockOutTime = isClockIn ? (TimeSpan?)null : DateTime.UtcNow.AddHours(3).TimeOfDay,
                    LastDetectedTime = DateTime.UtcNow
                };
                context.Attendances.Add(attendance);
                context.SaveChanges();
            }
            else
            {
                var attendance = context.Attendances.Where(a => a.StaffId == StaffId).OrderByDescending(a => a.Date).FirstOrDefault();
                if(isClockIn)
                {
                    attendance.ClockInTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                }
                else
                {
                    attendance.ClockOutTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                }
                attendance.LastDetectedTime = DateTime.UtcNow;
                context.SaveChanges();
            } 
        }   
       
    }

}
