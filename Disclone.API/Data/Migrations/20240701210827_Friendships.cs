using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Disclone.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Friendships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("10d4141c-462b-430d-b969-e28ca446d81c"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2d1e165e-623c-4393-a3b4-3fb6d668b993"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateModified",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 1, 21, 8, 26, 695, DateTimeKind.Utc).AddTicks(3725),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 6, 28, 18, 41, 41, 46, DateTimeKind.Utc).AddTicks(7275));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 1, 21, 8, 26, 695, DateTimeKind.Utc).AddTicks(3433),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 6, 28, 18, 41, 41, 46, DateTimeKind.Utc).AddTicks(6921));

            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    UserAId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserBId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => new { x.UserAId, x.UserBId });
                    table.ForeignKey(
                        name: "FK_Friendship_Users_UserAId",
                        column: x => x.UserAId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friendship_Users_UserBId",
                        column: x => x.UserBId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("7234e45a-1807-40e1-b76c-dfe7010fbefb"), null, "User", "USER" },
                    { new Guid("9d1dba7b-4d4e-43c8-8e83-0aa5d520e243"), null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserBId",
                table: "Friendship",
                column: "UserBId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendship");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("7234e45a-1807-40e1-b76c-dfe7010fbefb"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("9d1dba7b-4d4e-43c8-8e83-0aa5d520e243"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateModified",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 28, 18, 41, 41, 46, DateTimeKind.Utc).AddTicks(7275),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 7, 1, 21, 8, 26, 695, DateTimeKind.Utc).AddTicks(3725));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 28, 18, 41, 41, 46, DateTimeKind.Utc).AddTicks(6921),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 7, 1, 21, 8, 26, 695, DateTimeKind.Utc).AddTicks(3433));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("10d4141c-462b-430d-b969-e28ca446d81c"), null, "User", "USER" },
                    { new Guid("2d1e165e-623c-4393-a3b4-3fb6d668b993"), null, "Admin", "ADMIN" }
                });
        }
    }
}
