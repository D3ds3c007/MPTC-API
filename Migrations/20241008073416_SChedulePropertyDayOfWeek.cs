using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MPTC_API.Migrations
{
    /// <inheritdoc />
    public partial class SChedulePropertyDayOfWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the sequence in the public schema
            migrationBuilder.Sql(@"
                CREATE SEQUENCE public.matriculesequence
                START WITH 1
                INCREMENT BY 1
                NO MINVALUE
                NO MAXVALUE
                CACHE 1;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the sequence in the public schema if it exists
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS public.matriculesequence;");
        }
    }
}
