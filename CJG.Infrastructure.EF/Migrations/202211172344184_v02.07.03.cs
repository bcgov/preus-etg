using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.07.03")]
    public partial class v020703 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingPrograms", "CourseOutlineDocumentId", c => c.Int());
            CreateIndex("dbo.TrainingPrograms", "CourseOutlineDocumentId");
            AddForeignKey("dbo.TrainingPrograms", "CourseOutlineDocumentId", "dbo.Attachments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingPrograms", "CourseOutlineDocumentId", "dbo.Attachments");
            DropIndex("dbo.TrainingPrograms", new[] { "CourseOutlineDocumentId" });
            DropColumn("dbo.TrainingPrograms", "CourseOutlineDocumentId");
        }
    }
}
