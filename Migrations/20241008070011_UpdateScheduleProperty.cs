using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MPTC_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScheduleProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"Schedules\" " +
                "ALTER COLUMN \"End\" TYPE interval " +
                "USING \"End\"::time - '00:00:00'::time;"
            );

            migrationBuilder.Sql(
                "ALTER TABLE \"Schedules\" " +
                "ALTER COLUMN \"Begin\" TYPE interval " +
                "USING \"Begin\"::time - '00:00:00'::time;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Begin",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");
        }
    }
}
