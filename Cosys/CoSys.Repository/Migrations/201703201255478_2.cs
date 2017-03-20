namespace CoSys.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "PenName", c => c.String(maxLength: 32));
            AlterColumn("dbo.User", "IDCardAddres", c => c.String(maxLength: 256));
            AlterColumn("dbo.User", "IDCard", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "IDCard", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.User", "IDCardAddres", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.User", "PenName", c => c.String(nullable: false, maxLength: 32));
        }
    }
}
