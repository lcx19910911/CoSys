namespace CoSys.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "Addres", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User", "Addres", c => c.String(nullable: false, maxLength: 256));
        }
    }
}
