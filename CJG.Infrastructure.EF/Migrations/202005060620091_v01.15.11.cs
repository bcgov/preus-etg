namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.11")]
	public partial class v011511 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClassificationOfInstructionalPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                        Description = c.String(nullable: false, maxLength: 250),
                        Level = c.Int(nullable: false),
                        ParentId = c.Int(),
                        CIPSVersion = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassificationOfInstructionalPrograms", t => t.ParentId)
                .Index(t => new { t.Code, t.Level }, unique: true, name: "IX_ClassificationOfInstructionalPrograms")
                .Index(t => t.ParentId);
            
            AddColumn("dbo.TrainingPrograms", "TargetCipsCodeId", c => c.Int());
            CreateIndex("dbo.TrainingPrograms", "TargetCipsCodeId");
            AddForeignKey("dbo.TrainingPrograms", "TargetCipsCodeId", "dbo.ClassificationOfInstructionalPrograms", "Id");

			PreDeployment();
			PostDeployment();
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingPrograms", "TargetCipsCodeId", "dbo.ClassificationOfInstructionalPrograms");
            DropForeignKey("dbo.ClassificationOfInstructionalPrograms", "ParentId", "dbo.ClassificationOfInstructionalPrograms");
            DropIndex("dbo.ClassificationOfInstructionalPrograms", new[] { "ParentId" });
            DropIndex("dbo.ClassificationOfInstructionalPrograms", "IX_ClassificationOfInstructionalPrograms");
            DropIndex("dbo.TrainingPrograms", new[] { "TargetCipsCodeId" });
            DropColumn("dbo.TrainingPrograms", "TargetCipsCodeId");
            DropTable("dbo.ClassificationOfInstructionalPrograms");
        }
    }
}
