namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;
	[Description("v01.15.14")]
	public partial class v011514 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingPrograms", "CourseLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingPrograms", "CourseLink");
        }
    }
}
