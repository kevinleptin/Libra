namespace JinZhou.V2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addWxUserInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WxUserInfoes",
                c => new
                    {
                        OpenId = c.String(nullable: false, maxLength: 180),
                        NickName = c.String(maxLength: 180),
                        Sex = c.Int(nullable: false),
                        Country = c.String(maxLength: 50),
                        Province = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        HeadImgUrl = c.String(maxLength: 255),
                        UnionId = c.String(maxLength: 180),
                        AppId = c.String(maxLength: 180),
                    })
                .PrimaryKey(t => t.OpenId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WxUserInfoes");
        }
    }
}
