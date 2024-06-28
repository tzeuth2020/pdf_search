using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PdfSearch.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    batch = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    q1 = table.Column<string>(type: "text", nullable: true),
                    q2 = table.Column<string>(type: "text", nullable: true),
                    q3 = table.Column<string>(type: "text", nullable: true),
                    q4 = table.Column<string>(type: "text", nullable: true),
                    q5 = table.Column<string>(type: "text", nullable: true),
                    q6 = table.Column<string>(type: "text", nullable: true),
                    q7 = table.Column<string>(type: "text", nullable: true),
                    q8 = table.Column<string>(type: "text", nullable: true),
                    q9 = table.Column<string>(type: "text", nullable: true),
                    q10 = table.Column<string>(type: "text", nullable: true),
                    wholeDoc = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => new { x.batch, x.name });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submissions");
        }
    }
}
