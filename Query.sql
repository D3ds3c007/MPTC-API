-- Count Lateness per employee for a specific month

WITH months AS (
    SELECT 
        generate_series(
            date_trunc('month', (SELECT MIN("Date") FROM public."Attendances")),
            date_trunc('month', CURRENT_DATE),
            '1 month'
        ) AS month
)
SELECT 
    st."IdStaff" AS "StaffId",
    EXTRACT(YEAR FROM m.month) AS Year,  -- Extract year from month
    EXTRACT(MONTH FROM m.month) AS Month,  -- Extract month from month
    st."StaffName",
    COALESCE(COUNT(CASE 
            WHEN a."ClockInTime" IS NOT NULL 
              AND (EXTRACT(HOUR FROM a."ClockInTime") * interval '1 hour' + 
                   EXTRACT(MINUTE FROM a."ClockInTime") * interval '1 minute' > s."Begin") 
            THEN 1 
            ELSE NULL 
          END), 0) AS LatenessCount  -- Ensure 0 for no lateness
FROM 
    public."Staffs" st
CROSS JOIN months m  -- Create a cross join with all months
LEFT JOIN public."Attendances" a ON st."IdStaff" = a."StaffId"
    AND DATE_TRUNC('month', a."Date") = DATE_TRUNC('month', m.month)  -- Match attendance to the month
LEFT JOIN public."Schedules" s ON st."IdStaff" = s."StaffId"
    AND EXTRACT(DOW FROM a."Date") = s."DayOfWeek"  -- Ensure it's the same day of the week
WHERE 
    m.month = DATE '2024-10-01'  -- Change this to the desired month (e.g., January 2024)
GROUP BY st."IdStaff", Year, Month, st."StaffName"  -- Group by staff ID, year, month, and name
ORDER BY latenesscount DESC;  -- Order by staff ID

-- Count absences per employee for a specific month

WITH all_days AS (
    -- Generate all days in the specified month
    SELECT 
        generate_series(
            date_trunc('month', DATE '2024-01-01'),  -- Change this to the desired month
            date_trunc('month', DATE '2024-01-01') + interval '1 month' - interval '1 day',
            '1 day'
        ) AS day
)
SELECT 
    st."IdStaff" AS "StaffId",
    st."StaffName",
    COUNT(CASE 
            WHEN a."ClockInTime" IS NULL  -- No attendance record
                 AND NOT EXISTS (  -- Exclude if the day is covered by time off
                     SELECT 1 
                     FROM public."TimeOffs" t
                     WHERE t."StaffId" = st."IdStaff"
                       AND d.day BETWEEN t."BeginTimeOff" AND t."EndTimeOff"
                 ) THEN 1  -- Count as absence if no time off and no attendance
            ELSE NULL 
          END) AS AbsenceCount  -- Count of absences
FROM 
    public."Staffs" st
CROSS JOIN all_days d  -- Join with all days in the month
LEFT JOIN public."Schedules" s ON st."IdStaff" = s."StaffId"
    AND EXTRACT(DOW FROM d.day) = s."DayOfWeek"  -- Match the day of the week
LEFT JOIN public."Attendances" a ON st."IdStaff" = a."StaffId"
    AND a."Date" = d.day  -- Match attendance date with the generated day
WHERE 
    s."StaffId" IS NOT NULL  -- Only count employees with a schedule
    AND EXTRACT(MONTH FROM d.day) = 1  -- Ensure it's the correct month (January)
    AND EXTRACT(YEAR FROM d.day) = 2024  -- Ensure it's the correct year
GROUP BY 
    st."IdStaff", st."StaffName"
ORDER BY 
    st."IdStaff";


