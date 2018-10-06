using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JinZhou.Migrations
{
    public partial class initDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAuths",
                columns: table => new
                {
                    AppId = table.Column<string>(maxLength: 80, nullable: false),
                    Code = table.Column<string>(maxLength: 160, nullable: true),
                    ExpiredTime = table.Column<DateTime>(nullable: false),
                    Authorized = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAuths", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "BasicTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ticket = table.Column<string>(maxLength: 100, nullable: true),
                    TicketRefreshOn = table.Column<DateTime>(nullable: false),
                    AccessToken = table.Column<string>(maxLength: 160, nullable: true),
                    AccessTokenRefreshOn = table.Column<DateTime>(nullable: false),
                    AccessTokenExpiresIn = table.Column<int>(nullable: false),
                    PreAuthCode = table.Column<string>(maxLength: 80, nullable: true),
                    PreAuthCodeRefreshOn = table.Column<DateTime>(nullable: false),
                    PreAuthCodeExpiresIn = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicTokens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAuths");

            migrationBuilder.DropTable(
                name: "BasicTokens");
        }
    }
}
