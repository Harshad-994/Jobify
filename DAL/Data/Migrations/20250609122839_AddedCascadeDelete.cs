using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_JobCategories_JobCategoryId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_JobCategoryId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "JobCategoryId",
                table: "JobPostings");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_JobCategories_CategoryId",
                table: "JobPostings",
                column: "CategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_JobCategories_CategoryId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings");

            migrationBuilder.AddColumn<Guid>(
                name: "JobCategoryId",
                table: "JobPostings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_JobCategoryId",
                table: "JobPostings",
                column: "JobCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_JobCategories_JobCategoryId",
                table: "JobPostings",
                column: "JobCategoryId",
                principalTable: "JobCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
