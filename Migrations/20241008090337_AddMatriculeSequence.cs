using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MPTC_API.Migrations
{
    /// <inheritdoc />
    public partial class AddMatriculeSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the sequence if it exists
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS public.matriculesequence;");

            // Create the sequence in the public schema
            migrationBuilder.Sql(@"
                CREATE SEQUENCE public.matriculesequence
                START WITH 1000
                INCREMENT BY 1
                NO MINVALUE
                NO MAXVALUE
                CACHE 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the sequence in the public schema if it exists
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS public.matriculesequence;");

            // Optionally, recreate the original sequence if needed
            migrationBuilder.Sql(@"
                CREATE SEQUENCE MatriculeSequence
                START WITH 1000
                INCREMENT BY 1
                NO MINVALUE
                NO MAXVALUE
                CACHE 1;
            ");
        }
    }
}
