namespace CJG.Infrastructure.EF.Migrations
{
	using System.ComponentModel;

	[Description("v01.11.00")]
	public partial class v011100 : ExtendedDbMigration
	{
		public override void Up()
		{
			PreDeployment();

			DropPrimaryKey("dbo.PaymentRequests");
			CreateTable(
				"dbo.ClaimParticipants",
				c => new
					{
						Id = c.Int(nullable: false, identity: true),
						ClaimId = c.Int(nullable: false),
						ClaimVersion = c.Int(nullable: false),
						ParticipantFormId = c.Int(nullable: false),
						DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
						DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
						RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion }, cascadeDelete: true)
				.ForeignKey("dbo.ParticipantForms", t => t.ParticipantFormId, cascadeDelete: true)
				.Index(t => new { t.ClaimId, t.ClaimVersion })
				.Index(t => t.ParticipantFormId);
			
			AddColumn("dbo.Claims", "IsFinalClaim", c => c.Boolean(nullable: false));
			AddColumn("dbo.ClaimBreakdownCosts", "EligibleCostBreakdownId", c => c.Int(nullable: false));
			AddColumn("dbo.TrainingProviders", "OriginalTrainingProviderId", c => c.Int());
			AddColumn("dbo.TrainingProviders", "ChangeRequestReason", c => c.String(maxLength: 1000));
			AddColumn("dbo.ParticipantForms", "ReportedOn", c => c.DateTime());
			AlterColumn("dbo.GrantApplicationStateChanges", "Reason", c => c.String(maxLength: 2000));
			AddPrimaryKey("dbo.PaymentRequests", new[] { "PaymentRequestBatchId", "GrantApplicationId", "ClaimId" });
			CreateIndex("dbo.ClaimBreakdownCosts", "EligibleCostBreakdownId");
			CreateIndex("dbo.TrainingProviders", "OriginalTrainingProviderId");
			AddForeignKey("dbo.TrainingProviders", "OriginalTrainingProviderId", "dbo.TrainingProviders", "Id");
			AddForeignKey("dbo.ClaimBreakdownCosts", "EligibleCostBreakdownId", "dbo.EligibleCostBreakdowns", "Id", cascadeDelete: false);

			PostDeployment();
		}
		
		public override void Down()
		{
			DropForeignKey("dbo.ClaimParticipants", "ParticipantFormId", "dbo.ParticipantForms");
			DropForeignKey("dbo.ClaimBreakdownCosts", "EligibleCostBreakdownId", "dbo.EligibleCostBreakdowns");
			DropForeignKey("dbo.TrainingProviders", "OriginalTrainingProviderId", "dbo.TrainingProviders");
			DropForeignKey("dbo.ClaimParticipants", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
			DropIndex("dbo.TrainingProviders", new[] { "OriginalTrainingProviderId" });
			DropIndex("dbo.ClaimBreakdownCosts", new[] { "EligibleCostBreakdownId" });
			DropIndex("dbo.ClaimParticipants", new[] { "ParticipantFormId" });
			DropIndex("dbo.ClaimParticipants", new[] { "ClaimId", "ClaimVersion" });
			DropPrimaryKey("dbo.PaymentRequests");
			AlterColumn("dbo.GrantApplicationStateChanges", "Reason", c => c.String(maxLength: 1000));
			DropColumn("dbo.ParticipantForms", "ReportedOn");
			DropColumn("dbo.TrainingProviders", "ChangeRequestReason");
			DropColumn("dbo.TrainingProviders", "OriginalTrainingProviderId");
			DropColumn("dbo.ClaimBreakdownCosts", "EligibleCostBreakdownId");
			DropColumn("dbo.Claims", "IsFinalClaim");
			DropTable("dbo.ClaimParticipants");
			AddPrimaryKey("dbo.PaymentRequests", new[] { "PaymentRequestBatchId", "GrantApplicationId" });
		}
	}
}