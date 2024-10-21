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
    m.month = DATE '2024-01-01'  -- Change this to the desired month (e.g., January 2024)
GROUP BY st."IdStaff", Year, Month, st."StaffName"  -- Group by staff ID, year, month, and name
ORDER BY st."IdStaff";  -- Order by staff ID
