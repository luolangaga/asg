using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class stort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Integral",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "point",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StoreDB",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    information = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreDB", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "F_Store",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    buyerid = table.Column<long>(type: "bigint", nullable: false),
                    Storeid = table.Column<long>(type: "bigint", nullable: false),
                    isVerification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_Store", x => x.id);
                    table.ForeignKey(
                        name: "FK_F_Store_StoreDB_Storeid",
                        column: x => x.Storeid,
                        principalTable: "StoreDB",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F_Store_Storeid",
                table: "F_Store",
                column: "Storeid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F_Store");

            migrationBuilder.DropTable(
                name: "StoreDB");

            migrationBuilder.DropColumn(
                name: "Integral",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "point",
                table: "AspNetUsers");
        }
    }
}
