using Microsoft.EntityFrameworkCore.Migrations;

namespace JinZhou.Migrations
{
    public partial class fixAuthKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppAuths",
                table: "AppAuths");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizerAppId",
                table: "AppAuths",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "AppAuths",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 80);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppAuths",
                table: "AppAuths",
                column: "AuthorizerAppId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppAuths",
                table: "AppAuths");

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "AppAuths",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizerAppId",
                table: "AppAuths",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 80);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppAuths",
                table: "AppAuths",
                column: "AppId");
        }
    }
}
