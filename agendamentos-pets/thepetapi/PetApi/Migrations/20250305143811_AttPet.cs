using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetApi.Migrations
{
    /// <inheritdoc />
    public partial class AttPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailTutor",
                table: "Pets",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Tutor",
                table: "Pets",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailTutor",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Tutor",
                table: "Pets");
        }
    }
}
