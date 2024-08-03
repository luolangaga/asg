using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class combug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_F_ComForm_AspNetUsers_UserId",
                table: "F_ComForm");

            migrationBuilder.DropIndex(
                name: "IX_F_ComForm_UserId",
                table: "F_ComForm");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "F_ComForm",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "F_ComForm",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_F_ComForm_UserId",
                table: "F_ComForm",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_F_ComForm_AspNetUsers_UserId",
                table: "F_ComForm",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
