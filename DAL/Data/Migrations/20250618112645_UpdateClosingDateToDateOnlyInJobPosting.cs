using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.data.migrations
{
    /// <inheritdoc />
    public partial class UpdateClosingDateToDateOnlyInJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ClosingDate",
                table: "JobPostings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValueSql: "CURRENT_DATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ClosingDate",
                table: "JobPostings",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
