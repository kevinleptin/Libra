using Microsoft.EntityFrameworkCore.Migrations;

namespace JinZhou.Migrations
{
    public partial class addAutherInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizerUserName",
                table: "AppAuths",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuthorizerInfos",
                columns: table => new
                {
                    UserName = table.Column<string>(maxLength: 200, nullable: false),
                    NickName = table.Column<string>(maxLength: 80, nullable: true),
                    HeadImg = table.Column<string>(maxLength: 255, nullable: true),
                    ServiceType = table.Column<int>(nullable: false),
                    VerifyType = table.Column<int>(nullable: false),
                    PrincipalName = table.Column<string>(maxLength: 255, nullable: true),
                    BizStore = table.Column<int>(nullable: false),
                    BizScan = table.Column<int>(nullable: false),
                    BizPay = table.Column<int>(nullable: false),
                    BizCard = table.Column<int>(nullable: false),
                    BizShake = table.Column<int>(nullable: false),
                    Alias = table.Column<string>(maxLength: 200, nullable: true),
                    QrcodeUrl = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizerInfos", x => x.UserName);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAuths_AuthorizerUserName",
                table: "AppAuths",
                column: "AuthorizerUserName");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAuths_AuthorizerInfos_AuthorizerUserName",
                table: "AppAuths",
                column: "AuthorizerUserName",
                principalTable: "AuthorizerInfos",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAuths_AuthorizerInfos_AuthorizerUserName",
                table: "AppAuths");

            migrationBuilder.DropTable(
                name: "AuthorizerInfos");

            migrationBuilder.DropIndex(
                name: "IX_AppAuths_AuthorizerUserName",
                table: "AppAuths");

            migrationBuilder.DropColumn(
                name: "AuthorizerUserName",
                table: "AppAuths");
        }
    }
}
