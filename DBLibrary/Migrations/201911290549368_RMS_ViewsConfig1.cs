namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RMS_ViewsConfig1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_DepartmentID", "dbo.RMS_Department");
            DropForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DesignationID" });
            DropIndex("dbo.RIC_Employee", new[] { "RE_DepartmentID" });
            AddColumn("dbo.RMS_ViewsConfig", "Lk_ViewName", c => c.String(maxLength: 50));
            AddColumn("dbo.RMS_ViewsConfig", "LK_ColumnName", c => c.String(maxLength: 50));
            AlterColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int(nullable: false));
            AlterColumn("dbo.RIC_Employee", "RE_DepartmentID", c => c.Int(nullable: false));
            CreateIndex("dbo.RIC_Employee", "RE_DesignationID");
            CreateIndex("dbo.RIC_Employee", "RE_DepartmentID");
            AddForeignKey("dbo.RIC_Employee", "RE_DepartmentID", "dbo.RMS_Department", "RD_ID", cascadeDelete: true);
            AddForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation", "RD_ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation");
            DropForeignKey("dbo.RIC_Employee", "RE_DepartmentID", "dbo.RMS_Department");
            DropIndex("dbo.RIC_Employee", new[] { "RE_DepartmentID" });
            DropIndex("dbo.RIC_Employee", new[] { "RE_DesignationID" });
            AlterColumn("dbo.RIC_Employee", "RE_DepartmentID", c => c.Int());
            AlterColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int());
            DropColumn("dbo.RMS_ViewsConfig", "LK_ColumnName");
            DropColumn("dbo.RMS_ViewsConfig", "Lk_ViewName");
            CreateIndex("dbo.RIC_Employee", "RE_DepartmentID");
            CreateIndex("dbo.RIC_Employee", "RE_DesignationID");
            AddForeignKey("dbo.RIC_Employee", "RE_DesignationID", "dbo.RMS_Designation", "RD_ID");
            AddForeignKey("dbo.RIC_Employee", "RE_DepartmentID", "dbo.RMS_Department", "RD_ID");
        }
    }
}
