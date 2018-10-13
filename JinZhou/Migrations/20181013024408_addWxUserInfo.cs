using Microsoft.EntityFrameworkCore.Migrations;

namespace JinZhou.Migrations
{
    public partial class addWxUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WxUserInfos",
                columns: table => new
                {
                    OpenId = table.Column<string>(maxLength: 180, nullable: false),
                    NickName = table.Column<string>(maxLength: 180, nullable: true),
                    Sex = table.Column<int>(nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: true),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    HeadImgUrl = table.Column<string>(maxLength: 255, nullable: true),
                    UnionId = table.Column<string>(maxLength: 180, nullable: true),
                    AppId = table.Column<string>(maxLength: 180, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WxUserInfos", x => x.OpenId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WxUserInfos");
        }
    }
}
