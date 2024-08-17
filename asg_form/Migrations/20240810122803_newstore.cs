using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class newstore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_F_Store_StoreDB_Storeid",
                table: "F_Store");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreDB",
                table: "StoreDB");

            migrationBuilder.DropPrimaryKey(
                name: "PK_F_Store",
                table: "F_Store");

            migrationBuilder.RenameTable(
                name: "StoreDB",
                newName: "T_Store");

            migrationBuilder.RenameTable(
                name: "F_Store",
                newName: "T_Storeinfo");

            migrationBuilder.RenameIndex(
                name: "IX_F_Store_Storeid",
                table: "T_Storeinfo",
                newName: "IX_T_Storeinfo_Storeid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Store",
                table: "T_Store",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_Storeinfo",
                table: "T_Storeinfo",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Storeinfo_T_Store_Storeid",
                table: "T_Storeinfo",
                column: "Storeid",
                principalTable: "T_Store",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Storeinfo_T_Store_Storeid",
                table: "T_Storeinfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Storeinfo",
                table: "T_Storeinfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_T_Store",
                table: "T_Store");

            migrationBuilder.RenameTable(
                name: "T_Storeinfo",
                newName: "F_Store");

            migrationBuilder.RenameTable(
                name: "T_Store",
                newName: "StoreDB");

            migrationBuilder.RenameIndex(
                name: "IX_T_Storeinfo_Storeid",
                table: "F_Store",
                newName: "IX_F_Store_Storeid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_F_Store",
                table: "F_Store",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreDB",
                table: "StoreDB",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_F_Store_StoreDB_Storeid",
                table: "F_Store",
                column: "Storeid",
                principalTable: "StoreDB",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
