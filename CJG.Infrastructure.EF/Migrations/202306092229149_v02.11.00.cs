using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.11.00")]
    public partial class v021100 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingCosts", "TravelExpenseDocumentId", c => c.Int());
            CreateIndex("dbo.TrainingCosts", "TravelExpenseDocumentId");
            AddForeignKey("dbo.TrainingCosts", "TravelExpenseDocumentId", "dbo.Attachments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingCosts", "TravelExpenseDocumentId", "dbo.Attachments");
            DropIndex("dbo.TrainingCosts", new[] { "TravelExpenseDocumentId" });
            DropColumn("dbo.TrainingCosts", "TravelExpenseDocumentId");
        }
    }
}
