using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class friend1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F_Friend",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    headName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comMsg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    headTel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    comType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_Friend", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F_Friend");
        }
    }
}
