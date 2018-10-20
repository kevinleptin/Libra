namespace JinZhou.V2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMpInfos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MpInfoes",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 200),
                        NickName = c.String(maxLength: 80),
                        HeadImg = c.String(maxLength: 255),
                        ServiceType = c.Int(nullable: false),
                        VerifyType = c.Int(nullable: false),
                        PrincipalName = c.String(maxLength: 255),
                        BizStore = c.Int(nullable: false),
                        BizScan = c.Int(nullable: false),
                        BizPay = c.Int(nullable: false),
                        BizCard = c.Int(nullable: false),
                        BizShake = c.Int(nullable: false),
                        Alias = c.String(maxLength: 200),
                        QrcodeUrl = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.UserName);
            
            CreateTable(
                "dbo.MpTokens",
                c => new
                    {
                        MpAppId = c.String(nullable: false, maxLength: 180),
                        MpAccessToken = c.String(maxLength: 180),
                        MpRefreshToken = c.String(maxLength: 180),
                        ExpiredIn = c.Int(nullable: false),
                        RefreshOn = c.DateTime(nullable: false),
                        CreateOn = c.DateTime(nullable: false),
                        BelongToMpUserName = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.MpAppId)
                .ForeignKey("dbo.MpInfoes", t => t.BelongToMpUserName)
                .Index(t => t.BelongToMpUserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MpTokens", "BelongToMpUserName", "dbo.MpInfoes");
            DropIndex("dbo.MpTokens", new[] { "BelongToMpUserName" });
            DropTable("dbo.MpTokens");
            DropTable("dbo.MpInfoes");
        }
    }
}
