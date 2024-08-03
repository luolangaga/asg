using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class formupdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Common_Roles",
                table: "F_role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Game_Name",
                table: "F_role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Historical_Ranks",
                table: "F_role",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id_Card",
                table: "F_role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone_Number",
                table: "F_role",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Common_Roles",
                table: "F_role");

            migrationBuilder.DropColumn(
                name: "Game_Name",
                table: "F_role");

            migrationBuilder.DropColumn(
                name: "Historical_Ranks",
                table: "F_role");

            migrationBuilder.DropColumn(
                name: "Id_Card",
                table: "F_role");

            migrationBuilder.DropColumn(
                name: "Phone_Number",
                table: "F_role");
        }
    }
}
