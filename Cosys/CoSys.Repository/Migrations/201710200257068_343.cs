namespace CoSys.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _343 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "PlushMethodIDStr", c => c.String());
            DropColumn("dbo.News", "PlushMethodFlag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.News", "PlushMethodFlag", c => c.Long(nullable: false));
            DropColumn("dbo.News", "PlushMethodIDStr");
        }
    }
}
