using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.17.07")]
    public partial class v021707 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "PreviousEmploymentNaicsId", c => c.Int());
            CreateIndex("dbo.ParticipantForms", "PreviousEmploymentNaicsId");
            AddForeignKey("dbo.ParticipantForms", "PreviousEmploymentNaicsId", "dbo.NaIndustryClassificationSystems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParticipantForms", "PreviousEmploymentNaicsId", "dbo.NaIndustryClassificationSystems");
            DropIndex("dbo.ParticipantForms", new[] { "PreviousEmploymentNaicsId" });
            DropColumn("dbo.ParticipantForms", "PreviousEmploymentNaicsId");
        }
    }
}
