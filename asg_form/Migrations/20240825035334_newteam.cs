using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class newteam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "F_news",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "promChart",
                table: "F_events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "myteamId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "F_Team",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_check = table.Column<bool>(type: "bit", nullable: false),
                    piaoshu = table.Column<int>(type: "int", nullable: false),
                    time = table.Column<long>(type: "bigint", nullable: false),
                    team_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team_tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    logo_uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_Team", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_Config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Substance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    msg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "F_Player",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Game_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role_lin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id_Card = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Common_Roles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Id_Card_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Historical_Ranks = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_Player", x => x.Id);
                    table.ForeignKey(
                        name: "FK_F_Player_F_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "F_Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Teams_Player",
                columns: table => new
                {
                    EventsId = table.Column<int>(type: "int", nullable: false),
                    TeamsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Teams_Player", x => new { x.EventsId, x.TeamsId });
                    table.ForeignKey(
                        name: "FK_T_Teams_Player_F_Team_TeamsId",
                        column: x => x.TeamsId,
                        principalTable: "F_Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Teams_Player_F_events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "F_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_myteamId",
                table: "AspNetUsers",
                column: "myteamId");

            migrationBuilder.CreateIndex(
                name: "IX_F_Player_TeamId",
                table: "F_Player",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_T_Teams_Player_TeamsId",
                table: "T_Teams_Player",
                column: "TeamsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_F_Team_myteamId",
                table: "AspNetUsers",
                column: "myteamId",
                principalTable: "F_Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_F_Team_myteamId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "F_Player");

            migrationBuilder.DropTable(
                name: "T_Config");

            migrationBuilder.DropTable(
                name: "T_Teams_Player");

            migrationBuilder.DropTable(
                name: "F_Team");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_myteamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "F_news");

            migrationBuilder.DropColumn(
                name: "promChart",
                table: "F_events");

            migrationBuilder.DropColumn(
                name: "myteamId",
                table: "AspNetUsers");
        }
    }
}
