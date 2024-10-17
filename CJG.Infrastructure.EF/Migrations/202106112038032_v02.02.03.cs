namespace CJG.Infrastructure.EF.Migrations
{
	using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v02.02.03")]
	public partial class v020203 : ExtendedDbMigration
	{
		public override void Up()
		{
			// CJG-753
			CreateTable(
				"dbo.GrantStreamEligibilityQuestions",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					GrantStreamId = c.Int(nullable: false),
					EligibilityRequirements = c.String(maxLength: 2000),
					EligibilityQuestion = c.String(nullable: false, maxLength: 2000),
					IsActive = c.Boolean(nullable: false),
					EligibilityPositiveAnswerRequired = c.Boolean(nullable: false),
					RowSequence = c.Int(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.GrantStreams", t => t.GrantStreamId)
				.Index(t => t.GrantStreamId, name: "IX_GrantStreamEligibilityQuestions");

			CreateTable(
				"dbo.GrantStreamEligibilityAnswers",
				c => new
				{
					Id = c.Int(nullable: false, identity: true),
					GrantApplicationId = c.Int(nullable: false),
					GrantStreamEligibilityQuestionId = c.Int(nullable: false),
					EligibilityAnswer = c.Boolean(nullable: false),
					DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
					DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
					RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.GrantStreamEligibilityQuestions", t => t.GrantStreamEligibilityQuestionId, cascadeDelete: true)
				.ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
				.Index(t => new { t.GrantApplicationId, t.GrantStreamEligibilityQuestionId }, name: "IX_GrantStreamEligibilityAnswers");

			// CJG-687
			DropIndex("dbo.TrainingPeriods", "IX_TrainingPeriod");
			AddColumn("dbo.TrainingPeriods", "GrantStreamId", c => c.Int());
			AddColumn("dbo.TrainingPeriods", "IsActive", c => c.Boolean(nullable: false));
			CreateIndex("dbo.TrainingPeriods", new[] { "FiscalYearId", "StartDate", "EndDate", "GrantStreamId", "IsActive" }, unique: true, name: "IX_TrainingPeriod");
			AddForeignKey("dbo.TrainingPeriods", "GrantStreamId", "dbo.GrantStreams", "Id");

			// CJG-691
			AddColumn("dbo.TrainingPeriods", "OverpaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));

			PostDeployment();
		}

		public override void Down()
		{
			// CJG-753
			DropForeignKey("dbo.GrantStreamEligibilityAnswers", "GrantApplicationId", "dbo.GrantApplications");
			DropForeignKey("dbo.GrantStreamEligibilityAnswers", "GrantStreamEligibilityQuestionId", "dbo.GrantStreamEligibilityQuestions");
			DropForeignKey("dbo.GrantStreamEligibilityQuestions", "GrantStreamId", "dbo.GrantStreams");
			DropIndex("dbo.GrantStreamEligibilityAnswers", "IX_GrantStreamEligibilityAnswers");
			DropIndex("dbo.GrantStreamEligibilityQuestions", "IX_GrantStreamEligibilityQuestions");
			DropTable("dbo.GrantStreamEligibilityAnswers");
			DropTable("dbo.GrantStreamEligibilityQuestions");

			// CJG-687
			DropForeignKey("dbo.TrainingPeriods", "GrantStreamId", "dbo.GrantStreams");
			DropIndex("dbo.TrainingPeriods", "IX_TrainingPeriod");
			DropColumn("dbo.TrainingPeriods", "IsActive");
			DropColumn("dbo.TrainingPeriods", "GrantStreamId");
			//CreateIndex("dbo.TrainingPeriods", new[] { "FiscalYearId", "StartDate", "EndDate" }, unique: true, name: "IX_TrainingPeriod");

			// CJG-691
			DropColumn("dbo.TrainingPeriods", "OverpaymentAmount");
		}
	}
}
