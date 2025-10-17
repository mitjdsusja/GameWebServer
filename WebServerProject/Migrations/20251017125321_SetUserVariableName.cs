using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebServerProject.Migrations
{
    /// <inheritdoc />
    public partial class SetUserVariableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "users",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "TutorialCompleted",
                table: "users",
                newName: "tutorialCompleted");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "users",
                newName: "profileId");

            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "users",
                newName: "nickname");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "users",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Gold",
                table: "users",
                newName: "gold");

            migrationBuilder.RenameColumn(
                name: "Diamonds",
                table: "users",
                newName: "diamonds");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "tutorialCompleted",
                table: "Users",
                newName: "TutorialCompleted");

            migrationBuilder.RenameColumn(
                name: "profileId",
                table: "Users",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "nickname",
                table: "Users",
                newName: "Nickname");

            migrationBuilder.RenameColumn(
                name: "level",
                table: "Users",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "gold",
                table: "Users",
                newName: "Gold");

            migrationBuilder.RenameColumn(
                name: "diamonds",
                table: "Users",
                newName: "Diamonds");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}
