using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.data.migrations
{
    /// <inheritdoc />
    public partial class UpdateClosingDateToDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ClosingDate",
                table: "JobPostings",
                type: "date",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ClosingDate",
                table: "JobPostings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
