using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.17.05")]
	public partial class v021706 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "PreviousEmploymentNocId", c => c.Int());
            CreateIndex("dbo.ParticipantForms", "PreviousEmploymentNocId");
            AddForeignKey("dbo.ParticipantForms", "PreviousEmploymentNocId", "dbo.NationalOccupationalClassifications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParticipantForms", "PreviousEmploymentNocId", "dbo.NationalOccupationalClassifications");
            DropIndex("dbo.ParticipantForms", new[] { "PreviousEmploymentNocId" });
            DropColumn("dbo.ParticipantForms", "PreviousEmploymentNocId");
        }
    }
}
