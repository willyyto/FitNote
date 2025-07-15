using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitNote.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_Users_UserId1",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_UserId1",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserSubscriptions");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Workouts",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "OrderIndex",
                table: "WorkoutExercises",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "SetType",
                table: "ExerciseSets",
                newName: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Workouts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "WorkoutExercises",
                newName: "OrderIndex");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ExerciseSets",
                newName: "SetType");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserSubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId1",
                table: "UserSubscriptions",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_Users_UserId1",
                table: "UserSubscriptions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
