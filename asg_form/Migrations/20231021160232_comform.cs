using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class comform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F_ComForm",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Com_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Com_Cocial_media = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idv_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    introduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Com_qq = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_ComForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_F_ComForm_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F_ComForm_UserId",
                table: "F_ComForm",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F_ComForm");
        }
    }
}
