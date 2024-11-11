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

-- Step 4: Count absences and presences
SELECT 
    ap."IdStaff",
    ap."StaffName",
    COUNT(CASE WHEN ap.status = 'absent' THEN 1 END) AS AbsenceCount,  -- Count absences
    COUNT(CASE WHEN ap.status = 'present' THEN 1 END) AS PresenceCount  -- Count presences
FROM (
    SELECT 
        wd."IdStaff", 
        wd."StaffName", 
        wd.day,
        CASE 
            WHEN a."ClockInTime" IS NOT NULL THEN 'present'
            WHEN t."IdTimeOff" IS NOT NULL THEN 'timeoff'
            ELSE 'absent'
        END AS status
    FROM (
        SELECT 
            st."IdStaff", 
            st."StaffName",
            d.day
        FROM public."Staffs" st
        CROSS JOIN (
            SELECT generate_series(
                date_trunc('month', DATE '2024-10-01'), 
                date_trunc('month', DATE '2024-10-01') + interval '1 month' - interval '1 day',
                '1 day'
            ) AS day
        ) d
        JOIN public."Schedules" s 
            ON st."IdStaff" = s."StaffId" 
            AND EXTRACT(DOW FROM d.day) = s."DayOfWeek"
    ) wd
    LEFT JOIN public."Attendances" a 
        ON wd."IdStaff" = a."StaffId" 
        AND wd.day = DATE(a."Date")
    LEFT JOIN public."TimeOffs" t 
        ON wd."IdStaff" = t."StaffId" 
        AND wd.day BETWEEN t."BeginTimeOff" AND t."EndTimeOff"
) ap
GROUP BY ap."IdStaff", ap."StaffName"
ORDER BY ap."IdStaff";

-- Get list of employees with absences for a specific date

SELECT 
    st."IdStaff",
    st."StaffName",
    s."DayOfWeek",
    DATE '2024-10-24' AS "Date"
FROM public."Staffs" st
JOIN public."Schedules" s 
    ON st."IdStaff" = s."StaffId"
    AND s."DayOfWeek" = EXTRACT(DOW FROM DATE '2024-10-15')  -- Match the day of the week for the specific date
LEFT JOIN public."Attendances" a 
    ON st."IdStaff" = a."StaffId" 
    AND DATE(a."Date") = DATE '2024-10-24'  -- Check for attendance on the specific date
LEFT JOIN public."TimeOffs" t 
    ON st."IdStaff" = t."StaffId" 
    AND DATE '2024-10-24' BETWEEN t."BeginTimeOff" AND t."EndTimeOff"  -- Check if the employee had time off on that date
WHERE 
    a."IdAttendance" IS NULL  -- No attendance record for the date
    AND t."IdTimeOff" IS NULL  -- No time-off record for the date
ORDER BY 
    st."IdStaff";



-- Combination
WITH months AS (
    SELECT 
        generate_series(
            date_trunc('month', (SELECT MIN("Date") FROM public."Attendances")),
            date_trunc('month', CURRENT_DATE),
            '1 month'
        ) AS month
),
-- Lateness Count Subquery
lateness AS (
    SELECT 
        st."IdStaff" AS "StaffId",
        EXTRACT(YEAR FROM m.month) AS Year,
        EXTRACT(MONTH FROM m.month) AS Month,
        st."StaffName",
        COUNT(CASE 
            WHEN a."ClockInTime" IS NOT NULL 
              AND (EXTRACT(HOUR FROM a."ClockInTime") * interval '1 hour' + 
                   EXTRACT(MINUTE FROM a."ClockInTime") * interval '1 minute' > s."Begin") 
            THEN 1 
            ELSE NULL 
          END) AS LatenessCount
    FROM 
        public."Staffs" st
    CROSS JOIN months m
    LEFT JOIN public."Attendances" a ON st."IdStaff" = a."StaffId"
        AND DATE_TRUNC('month', a."Date") = DATE_TRUNC('month', m.month)
    LEFT JOIN public."Schedules" s ON st."IdStaff" = s."StaffId"
        AND EXTRACT(DOW FROM a."Date") = s."DayOfWeek"
    WHERE 
        m.month = DATE '2024-11-01'  -- Change this to the desired month
    GROUP BY st."IdStaff", Year, Month, st."StaffName"
),
-- Absence Count Subquery
absence AS (
    SELECT 
        wd."IdStaff", 
        wd."StaffName", 
        COUNT(CASE WHEN a."ClockInTime" IS NULL AND t."IdTimeOff" IS NULL THEN 1 END) AS AbsenceCount
    FROM (
        SELECT 
            st."IdStaff", 
            st."StaffName",
            d.day
        FROM public."Staffs" st
        CROSS JOIN (
            SELECT generate_series(
                date_trunc('month', DATE '2024-11-01'), 
                date_trunc('month', DATE '2024-11-01') + interval '1 month' - interval '1 day',
                '1 day'
            ) AS day
        ) d
        JOIN public."Schedules" s 
            ON st."IdStaff" = s."StaffId" 
            AND EXTRACT(DOW FROM d.day) = s."DayOfWeek"
    ) wd
    LEFT JOIN public."Attendances" a 
        ON wd."IdStaff" = a."StaffId" 
        AND wd.day = DATE(a."Date")
    LEFT JOIN public."TimeOffs" t 
        ON wd."IdStaff" = t."StaffId" 
        AND wd.day BETWEEN t."BeginTimeOff" AND t."EndTimeOff"
    GROUP BY wd."IdStaff", wd."StaffName"
),
-- On Time Count Subquery
ontime AS (
    SELECT 
        st."IdStaff" AS "StaffId",
        EXTRACT(YEAR FROM m.month) AS Year,
        EXTRACT(MONTH FROM m.month) AS Month,
        COUNT(CASE 
            WHEN a."ClockInTime" IS NOT NULL 
              AND (EXTRACT(HOUR FROM a."ClockInTime") * interval '1 hour' + 
                   EXTRACT(MINUTE FROM a."ClockInTime") * interval '1 minute' <= s."Begin") 
            THEN 1 
            ELSE NULL 
          END) AS OnTimeCount
    FROM 
        public."Staffs" st
    CROSS JOIN months m
    LEFT JOIN public."Attendances" a ON st."IdStaff" = a."StaffId"
        AND DATE_TRUNC('month', a."Date") = DATE_TRUNC('month', m.month)
    LEFT JOIN public."Schedules" s ON st."IdStaff" = s."StaffId"
        AND EXTRACT(DOW FROM a."Date") = s."DayOfWeek"
    WHERE 
        m.month = DATE '2024-11-01'  -- Change this to the desired month
    GROUP BY st."IdStaff", Year, Month
)
-- Combine Lateness, Absence, and On Time Counts with Punctuality Rating
SELECT 
    s."IdStaff" AS "StaffId",
    s."StaffName",
    s."Matricule",
    COALESCE(l.Year, EXTRACT(YEAR FROM DATE '2024-11-01')) AS Year,
    COALESCE(l.Month, EXTRACT(MONTH FROM DATE '2024-11-01')) AS Month,
    COALESCE(l."latenesscount", 0) AS LatenessCount,
    COALESCE(a."absencecount", 0) AS AbsenceCount,
    COALESCE(ot."ontimecount", 0) AS OnTimeCount,
    CASE 
        WHEN COALESCE(l."latenesscount", 0) = 0 AND COALESCE(a."absencecount", 0) = 0 THEN 'Excellent'
        WHEN COALESCE(l."latenesscount", 0) <= 2 AND COALESCE(a."absencecount", 0) <= 1 THEN 'Good'
        WHEN COALESCE(l."latenesscount", 0) <= 4 AND COALESCE(a."absencecount", 0) <= 2 THEN 'Average'
        WHEN COALESCE(l."latenesscount", 0) <= 6 AND COALESCE(a."absencecount", 0) <= 3 THEN 'Fair'
        ELSE 'Poor'
    END AS PunctualityRating
FROM public."Staffs" s
LEFT JOIN lateness l ON s."IdStaff" = l."StaffId"
LEFT JOIN absence a ON s."IdStaff" = a."IdStaff"
LEFT JOIN ontime ot ON s."IdStaff" = ot."StaffId"
ORDER BY "StaffId";


-- AVG lateness GLOBAL per Employee

WITH lateness AS (
    SELECT 
        st."IdStaff" AS "StaffId",
        EXTRACT(YEAR FROM a."Date") AS Year,
        EXTRACT(MONTH FROM a."Date") AS Month,
        st."StaffName",
        COUNT(CASE 
            WHEN a."ClockInTime" IS NOT NULL 
            AND (EXTRACT(HOUR FROM a."ClockInTime") * interval '1 hour' + 
                EXTRACT(MINUTE FROM a."ClockInTime") * interval '1 minute' > s."Begin") 
            THEN 1 
            ELSE NULL 
        END) AS LatenessCount,
        AVG(
            CASE 
                WHEN a."ClockInTime" IS NOT NULL 
                AND (EXTRACT(HOUR FROM a."ClockInTime") * interval '1 hour' + 
                     EXTRACT(MINUTE FROM a."ClockInTime") * interval '1 minute' > s."Begin") 
                THEN EXTRACT(EPOCH FROM (a."ClockInTime" - (s."Begin"::time)))
                ELSE NULL
            END
        ) / 60 AS AvgLatenessDurationMinutes -- Average lateness duration in minutes
    FROM    
        public."Staffs" st
    LEFT JOIN public."Attendances" a ON st."IdStaff" = a."StaffId"
    LEFT JOIN public."Schedules" s ON st."IdStaff" = s."StaffId"
        AND EXTRACT(DOW FROM a."Date") = s."DayOfWeek"
    GROUP BY st."IdStaff", Year, Month, st."StaffName"
),
absence AS (
    SELECT 
        wd."IdStaff", 
        wd."StaffName", 
        COUNT(CASE WHEN a."ClockInTime" IS NULL AND t."IdTimeOff" IS NULL THEN 1 END) AS AbsenceCount
    FROM (
        SELECT 
            st."IdStaff", 
            st."StaffName",
            d.day
        FROM public."Staffs" st
        CROSS JOIN (
            SELECT generate_series(
                '2024-01-01'::date,  -- Start date of all data
                CURRENT_DATE,  -- Current date
                '1 day'
            ) AS day
        ) d
        JOIN public."Schedules" s 
            ON st."IdStaff" = s."StaffId" 
            AND EXTRACT(DOW FROM d.day) = s."DayOfWeek"
    ) wd
    LEFT JOIN public."Attendances" a 
        ON wd."IdStaff" = a."StaffId" 
        AND wd.day = DATE(a."Date")
    LEFT JOIN public."TimeOffs" t 
        ON wd."IdStaff" = t."StaffId" 
        AND wd.day BETWEEN t."BeginTimeOff" AND t."EndTimeOff"
    GROUP BY wd."IdStaff", wd."StaffName"
)
SELECT 
    s."IdStaff" AS "StaffId",
    s."StaffName",
    l.Year,
    l.Month,
    COALESCE(l."avglatenessdurationminutes", 0) AS AvgLatenessDurationMinutes
FROM public."Staffs" s
LEFT JOIN lateness l ON s."IdStaff" = l."StaffId"
LEFT JOIN absence a ON s."IdStaff" = a."IdStaff"
ORDER BY l.Year DESC, l.Month DESC, AvgLatenessDurationMinutes DESC;

-- AVG of Duration

SELECT 
    AVG(AvgLatenessDurationMinutes) AS AvgLatenessDurationMinutes
FROM 
public."v_LatenessDurationAvg" as v
WHERE v.year = 2024 AND v.month = 11;


-- List Of activity Logs view
SELECT
    s."Matricule",
    CONCAT(s."StaffName", ' ', s."FirstName") as FirstName, 
    l."EventType",
    EXTRACT(YEAR FROM l."EventTime") AS Year,
    EXTRACT(MONTH FROM l."EventTime") AS Month,

    l."EventTime" FROM public."Logss" l
LEFT JOIN public."Staffs" s ON s."IdStaff" = l."StaffId"






