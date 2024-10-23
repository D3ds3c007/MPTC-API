using Microsoft.AspNetCore.SignalR;
using MPTC_API.Data;
using MPTC_API.Hub;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;


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

            if(lastDetectedTime == null || ((currentTime-lastDetectedTime.Value).TotalMinutes >= 1440 && lastDetectedTime.Value.Date != currentTime.Date))
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
                await context.SaveChangesAsync();
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
                await context.SaveChangesAsync();
            } 
        }

        //static function to map list of attendance to list of attendanceDTO and check if the staff is late
        public static List<AttendanceDTO> MapAttendanceToDTO(List<MPTC_API.Models.Attendance.Attendance> attendances)
        {
            List<AttendanceDTO> attendanceDTOs = new List<AttendanceDTO>();
            foreach (var attendance in attendances)
            {
                
                AttendanceDTO attendanceDTO = new AttendanceDTO
                {
                    AttendanceId = attendance.IdAttendance,
                    Matricule = attendance.Staff.Matricule,
                    StaffName = attendance.Staff.FirstName + " " + attendance.Staff.StaffName,
                    recordDate = attendance.Date,
                    timeIn = attendance.ClockInTime,
                    timeOut = attendance.ClockOutTime,
                    isLate = IsLate(attendance),
                    remark = attendance.Remark ?? "N/A"
                };
                attendanceDTOs.Add(attendanceDTO);
            }
            return attendanceDTOs;
        }  

        //function to check if the staff is late based on it's schedule
        public static  bool IsLate(MPTC_API.Models.Attendance.Attendance attendance)
        {
            if(attendance.ClockInTime != null)
            {
                TimeSpan timeIn = attendance.ClockInTime.Value;

                TimeSpan? begin = attendance.Staff.Schedules.FirstOrDefault(s => s.DayOfWeek == attendance.Date.DayOfWeek)?.Begin;

                //return false if begin is null
                if(begin == null)
                {
                    return false;
                }
                
                return timeIn > begin;
            }
            return false;
        }
       
    }

}
