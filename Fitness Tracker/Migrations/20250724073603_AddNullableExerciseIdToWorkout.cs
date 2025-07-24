using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_Tracker.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableExerciseIdToWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exercise",
                table: "Workouts");

            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "Workouts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_ExerciseId",
                table: "Workouts",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Exercises_ExerciseId",
                table: "Workouts",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Exercises_ExerciseId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_ExerciseId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "Workouts");

            migrationBuilder.AddColumn<string>(
                name: "Exercise",
                table: "Workouts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
