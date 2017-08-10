namespace CoSys.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.News", "NewsTypeID", c => c.String(maxLength: 32, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.News", "NewsTypeID", c => c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false));
        }
    }
}
