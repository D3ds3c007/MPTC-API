using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MPTC_API.Data;
using MPTC_API.Hub;
using MPTC_API.Migrations;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.Attendance.MemberDTO;
using MPTC_API.Models.DTO;
using MPTC_API.Models.StaffDTO;
using Npgsql;



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
                // if(isClockIn)
                // {
                //     attendance.ClockInTime = DateTime.UtcNow.AddHours(3).TimeOfDay;
                // }
                if(!isClockIn)
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

        public static async Task<IEnumerable<StaffScheduleDTO>> GetAbsenceAsync(DateTime date, MptcContext _context)
        {
            //parse date to the format (yyyy-MM-dd)
            string dateStr = date.ToString("yyyy-MM-dd");

            Console.WriteLine("Date is : " + dateStr);
            var result = await _context.StaffScheduleDTOs
            .FromSqlRaw(@$"
                    SELECT
                        st.""IdStaff"",
                        st.""Matricule"", 
                        st.""StaffName"",
                        s.""DayOfWeek"",
                        DATE '{dateStr}' AS ""Date""
                    FROM public.""Staffs"" st
                    JOIN public.""Schedules"" s 
                        ON st.""IdStaff"" = s.""StaffId""
                        AND s.""DayOfWeek"" = EXTRACT(DOW FROM DATE '{dateStr}')
                    LEFT JOIN public.""Attendances"" a 
                        ON st.""IdStaff"" = a.""StaffId"" 
                        AND DATE(a.""Date"") = DATE '{dateStr}'
                    LEFT JOIN public.""TimeOffs"" t 
                        ON st.""IdStaff"" = t.""StaffId"" 
                        AND DATE '{dateStr}' BETWEEN t.""BeginTimeOff"" AND t.""EndTimeOff""
                    WHERE 
                        a.""IdAttendance"" IS NULL
                        AND t.""IdTimeOff"" IS NULL
                        AND DATE '{dateStr}' <= CURRENT_DATE
                    ORDER BY 
                        st.""IdStaff""")
                .ToListAsync();

            return result;
        }

        public static async Task<List<Models.DTO.LeaderboardDTO>> GetLeaderboardAsync(int month, MptcContext _context)
        {
            var targetDate = new DateTime(DateTime.Now.Year, month, 1);
            var result = await _context.LeaderboardDTOs
                .FromSqlRaw(@"
                    WITH months AS (
                        SELECT generate_series(
                            date_trunc('month', (SELECT MIN(""Date"") FROM public.""Attendances"")),
                            date_trunc('month', CURRENT_DATE),
                            '1 month'
                        ) AS month
                    ),
                    lateness AS (
                        SELECT 
                            st.""IdStaff"" AS ""StaffId"",
                            EXTRACT(YEAR FROM m.month) AS Year,
                            EXTRACT(MONTH FROM m.month) AS Month,
                            st.""StaffName"",
                            COUNT(CASE 
                                WHEN a.""ClockInTime"" IS NOT NULL 
                                AND a.""ClockInTime"" > s.""Begin""
                                THEN 1 
                                ELSE NULL 
                            END) AS latenesscount
                        FROM 
                            public.""Staffs"" st
                        CROSS JOIN months m
                        LEFT JOIN public.""Attendances"" a ON st.""IdStaff"" = a.""StaffId""
                                AND DATE_TRUNC('month', a.""Date"") = DATE_TRUNC('month', m.month)
                        LEFT JOIN public.""Schedules"" s ON st.""IdStaff"" = s.""StaffId""
                                AND EXTRACT(DOW FROM a.""Date"") = s.""DayOfWeek""
                        WHERE 
                            m.month = @Date
                        GROUP BY st.""IdStaff"", Year, Month, st.""StaffName""
                    ),
                    absence AS (
                        SELECT 
                            wd.""IdStaff"", 
                            wd.""StaffName"", 
                            COUNT(CASE WHEN a.""ClockInTime"" IS NULL AND t.""IdTimeOff"" IS NULL THEN 1 END) AS AbsenceCount
                        FROM (
                            SELECT 
                                st.""IdStaff"", 
                                st.""StaffName"",
                                d.day
                            FROM public.""Staffs"" st
                            CROSS JOIN (
                                SELECT generate_series(
                                    date_trunc('month', @Date), 
                                    date_trunc('month', @Date) + interval '1 month' - interval '1 day',
                                    '1 day'
                                ) AS day
                            ) d
                            JOIN public.""Schedules"" s 
                                ON st.""IdStaff"" = s.""StaffId"" 
                                AND EXTRACT(DOW FROM d.day) = s.""DayOfWeek""
                        ) wd
                        LEFT JOIN public.""Attendances"" a 
                            ON wd.""IdStaff"" = a.""StaffId"" 
                            AND DATE(a.""Date"") = wd.day
                        LEFT JOIN public.""TimeOffs"" t 
                            ON wd.""IdStaff"" = t.""StaffId"" 
                            AND wd.day BETWEEN t.""BeginTimeOff"" AND t.""EndTimeOff""
                        
                        GROUP BY wd.""IdStaff"", wd.""StaffName""
                    ),
                    ontime AS (
                        SELECT 
                            st.""IdStaff"" AS ""StaffId"",
                            EXTRACT(YEAR FROM m.month) AS Year,
                            EXTRACT(MONTH FROM m.month) AS Month,
                            st.""StaffName"",
                            COUNT(CASE 
                                WHEN a.""ClockInTime"" IS NOT NULL 
                                AND a.""ClockInTime"" <= s.""Begin""
                                THEN 1 
                                ELSE NULL 
                            END) AS OnTimeCount
                        FROM 
                            public.""Staffs"" st
                        CROSS JOIN months m
                        LEFT JOIN public.""Attendances"" a ON st.""IdStaff"" = a.""StaffId""
                                AND DATE_TRUNC('month', a.""Date"") = DATE_TRUNC('month', m.month)
                        LEFT JOIN public.""Schedules"" s ON st.""IdStaff"" = s.""StaffId""
                                AND EXTRACT(DOW FROM a.""Date"") = s.""DayOfWeek""
                        WHERE 
                            m.month = @Date
                        GROUP BY st.""IdStaff"", Year, Month, st.""StaffName""
                    )
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY 
                            CASE 
                                WHEN COALESCE(l.""latenesscount"", 0) = 0 AND COALESCE(a.""absencecount"", 0) = 0 THEN 1
                                WHEN COALESCE(l.""latenesscount"", 0) <= 2 AND COALESCE(a.""absencecount"", 0) <= 1 THEN 2
                                WHEN COALESCE(l.""latenesscount"", 0) <= 4 AND COALESCE(a.""absencecount"", 0) <= 2 THEN 3
                                WHEN COALESCE(l.""latenesscount"", 0) <= 6 AND COALESCE(a.""absencecount"", 0) <= 3 THEN 4
                                ELSE 5
                            END, 
                            l.""latenesscount"", a.""absencecount""
                        ) AS rank,
                        s.""IdStaff"" AS ""StaffId"",
                        s.""StaffName"",
                        s.""Matricule"",
                        COALESCE(l.Year, EXTRACT(YEAR FROM @Date)) AS Year,
                        COALESCE(l.Month, EXTRACT(MONTH FROM @Date)) AS Month,
                        COALESCE(l.""latenesscount"", 0) AS latenesscount,
                        COALESCE(a.""absencecount"", 0) AS absencecount,
                        COALESCE(ot.""ontimecount"", 0) AS ontimecount,
                        CASE 
                            WHEN COALESCE(l.""latenesscount"", 0) = 0 AND COALESCE(a.""absencecount"", 0) = 0 THEN 'Excellent'
                            WHEN COALESCE(l.""latenesscount"", 0) <= 2 AND COALESCE(a.""absencecount"", 0) <= 1 THEN 'Good'
                            WHEN COALESCE(l.""latenesscount"", 0) <= 4 AND COALESCE(a.""absencecount"", 0) <= 2 THEN 'Average'
                            WHEN COALESCE(l.""latenesscount"", 0) <= 6 AND COALESCE(a.""absencecount"", 0) <= 3 THEN 'Fair'
                            ELSE 'Poor'
                        END AS PunctualityRating
                    FROM public.""Staffs"" s
                    LEFT JOIN lateness l ON s.""IdStaff"" = l.""StaffId""
                    LEFT JOIN absence a ON s.""IdStaff"" = a.""IdStaff""
                    LEFT JOIN ontime ot ON s.""IdStaff"" = ot.""StaffId""
                    ORDER BY rank;
                ", new NpgsqlParameter("@Date", targetDate)).ToListAsync();

    
            //remove from result where lateness is 0 and punnctuality 0 and readjust the rank
            result = result.Where(r => r.LatenessCount != 0 || r.AbsenceCount != 0).ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].Rank = i + 1;
            }
            
            return result;
        }

        public static async Task<List<ActivityLogDTO>> GetActivityLogsAsync(DateTime? date, MptcContext _context)
        {

            //if the date is null, set it to the current date
            if(date == null) date = DateTime.UtcNow;
            var dateString = date.Value.ToString("yyyy-MM-dd");
             var query = $@"
                SELECT * FROM public.""v_Logs"" AS l WHERE l.year = {DateTime.UtcNow.Year} AND l.""EventTime""::DATE = '{dateString}';
            ";

            var result = _context.Set<ActivityLogDTO>()
                                .FromSqlRaw(query)
                                .AsEnumerable()
                                .ToList(); 

            return result; 
        }

       
    }

}
