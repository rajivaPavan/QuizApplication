using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizApplication.Migrations
{
    public partial class UpdatedAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestions_AnswerOptions_SelectedAnswerOptionId",
                table: "QuizQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuizQuestions_SelectedAnswerOptionId",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "SelectedAnswerOptionId",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnswerOptions");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "QuizQuestions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnswerNo",
                table: "AnswerOptions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer",
                table: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "AnswerNo",
                table: "AnswerOptions");

            migrationBuilder.AddColumn<int>(
                name: "SelectedAnswerOptionId",
                table: "QuizQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Questions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AnswerOptions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_SelectedAnswerOptionId",
                table: "QuizQuestions",
                column: "SelectedAnswerOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestions_AnswerOptions_SelectedAnswerOptionId",
                table: "QuizQuestions",
                column: "SelectedAnswerOptionId",
                principalTable: "AnswerOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
