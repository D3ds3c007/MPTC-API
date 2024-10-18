using Microsoft.AspNetCore.SignalR;
using MPTC_API.Data;
using MPTC_API.Hub;
using MPTC_API.Models.Attendance;


namespace MPTC_API.Services.Attendance
{
    public class AttendanceService
    {
        

       
        public static void LogAttendance(int StaffId, bool isClockIn, MptcContext context, IHubContext<AttendanceHub> _hubContext)
        {
            Console.WriteLine("Loging attendance");
            var recentLog = context.Logss
                .Where(l => l.StaffId == StaffId)
                .OrderByDescending(l => l.EventTime)
                .FirstOrDefault();
            DateTime current = DateTime.UtcNow;
            //Check the event type and its delay between the current time, Add the log to database if delay is superior 1min
            if (recentLog == null || (isClockIn && recentLog.EventType == "ClockOut" && (current - recentLog.EventTime).TotalMinutes >= 1) || (!isClockIn && recentLog.EventType == "ClockIn" && (current - recentLog.EventTime).TotalMinutes >= 1))
            {
                Log log = new Log
                {
                    StaffId = StaffId,
                    EventType = isClockIn ? "ClockIn" : "ClockOut",
                    EventTime = DateTime.UtcNow
                };
                context.Logss.Add(log);
                context.SaveChanges();
            }

            // After saving, fetch the recent two logs and include only the staff object without joining other entity
            var recentLogs = context.Logss
                .OrderByDescending(l => l.EventTime)
                .Take(2)
                .Select(l => new
                {
                    l.IdLogs,
                    l.EventType,
                    l.EventTime,
                    Staff = new
                    {
                        l.Staff.IdStaff,
                        l.Staff.FirstName,
                        l.Staff.Privilege.PrivilegeName
                    }
                })
                .ToList();
      

            // Use the hub context to send the recent activities to the clients
            _hubContext.Clients.All.SendAsync("ReceiveRecentActivities", recentLogs);
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

            if(lastDetectedTime == null || ((currentTime-lastDetectedTime.Value).TotalMinutes >= 10 && lastDetectedTime.Value.Date != currentTime.Date))
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
