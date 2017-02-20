namespace CoSys.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "NewsDepartmentID", c => c.String(nullable: false, maxLength: 512));
            AddColumn("dbo.NewsDepartment", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NewsDepartment", "IsDelete");
            DropColumn("dbo.News", "NewsDepartmentID");
        }
    }
}
