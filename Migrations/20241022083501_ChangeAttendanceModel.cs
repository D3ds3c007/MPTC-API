using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MPTC_API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttendanceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "Attendances",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                table: "Attendances");
        }
    }
}
