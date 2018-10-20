namespace JinZhou.V2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeAccessTokenLen : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ComponentTokens", "ComponentAccessToken", c => c.String(maxLength: 160));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ComponentTokens", "ComponentAccessToken", c => c.String(maxLength: 100));
        }
    }
}
