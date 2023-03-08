using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizApplication.Migrations
{
    public partial class QuizAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionNo",
                table: "QuizQuestions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "QuizQuestions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "QuizQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionNo",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "QuizQuestions");
        }
    }
}
