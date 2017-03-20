namespace CoSys.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "DepartmentFlag", c => c.Long(nullable: false));
            AddColumn("dbo.User", "OperateFlag", c => c.Long(nullable: false));
            AddColumn("dbo.User", "RoleID", c => c.String(maxLength: 32, fixedLength: true, unicode: false));
            AddColumn("dbo.User", "IsSuperAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "IsAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "AuditCount", c => c.Int(nullable: false));
            AddColumn("dbo.User", "EditCount", c => c.Int(nullable: false));
            AddColumn("dbo.User", "AuditPassCount", c => c.Int(nullable: false));
            AddColumn("dbo.User", "PlushCount", c => c.Int(nullable: false));
            DropColumn("dbo.News", "AdminID");
            DropTable("dbo.Admin");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Admin",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false),
                        DepartmentFlag = c.Long(nullable: false),
                        OperateFlag = c.Long(nullable: false),
                        RoleID = c.String(nullable: false, maxLength: 32, fixedLength: true, unicode: false),
                        IsSuperAdmin = c.Boolean(nullable: false),
                        Account = c.String(nullable: false, maxLength: 12, unicode: false),
                        Name = c.String(nullable: false, maxLength: 32, unicode: false),
                        Password = c.String(nullable: false, maxLength: 128, unicode: false),
                        Remark = c.String(maxLength: 128, unicode: false),
                        ProvoniceCode = c.String(maxLength: 32),
                        CityCode = c.String(maxLength: 32),
                        CountyCode = c.String(maxLength: 32),
                        StreetCode = c.String(maxLength: 32),
                        AuditCount = c.Int(nullable: false),
                        EditCount = c.Int(nullable: false),
                        AuditPassCount = c.Int(nullable: false),
                        PlushCount = c.Int(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedTime = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.News", "AdminID", c => c.String(maxLength: 32, fixedLength: true, unicode: false));
            DropColumn("dbo.User", "PlushCount");
            DropColumn("dbo.User", "AuditPassCount");
            DropColumn("dbo.User", "EditCount");
            DropColumn("dbo.User", "AuditCount");
            DropColumn("dbo.User", "IsAdmin");
            DropColumn("dbo.User", "IsSuperAdmin");
            DropColumn("dbo.User", "RoleID");
            DropColumn("dbo.User", "OperateFlag");
            DropColumn("dbo.User", "DepartmentFlag");
        }
    }
}
