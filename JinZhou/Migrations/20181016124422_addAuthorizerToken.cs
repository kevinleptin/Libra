using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JinZhou.Migrations
{
    public partial class addAuthorizerToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizerTokens",
                columns: table => new
                {
                    AuthorizerAppId = table.Column<string>(maxLength: 180, nullable: false),
                    AuthorizerAccessToken = table.Column<string>(maxLength: 180, nullable: true),
                    AuthorizerRefreshToken = table.Column<string>(maxLength: 180, nullable: true),
                    ExpiredIn = table.Column<int>(nullable: false),
                    RefreshOn = table.Column<DateTime>(nullable: false),
                    CreateOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizerTokens", x => x.AuthorizerAppId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizerTokens");
        }
    }
}
