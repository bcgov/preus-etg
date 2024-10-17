using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.08.00")]
    public partial class v020800 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrioritizationScoreBreakdowns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IndustryScore = c.Int(nullable: false),
                        IndustryName = c.String(),
                        IndustryCode = c.String(),
                        RegionalScore = c.Int(nullable: false),
                        RegionalName = c.String(),
                        SmallBusinessScore = c.Int(nullable: false),
                        FirstTimeApplicantScore = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrioritizationScoreBreakdownAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionScore = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        GrantStreamEligibilityQuestionId = c.Int(nullable: false),
                        PrioritizationScoreBreakdownId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrantStreamEligibilityQuestions", t => t.GrantStreamEligibilityQuestionId)
                .ForeignKey("dbo.PrioritizationScoreBreakdowns", t => t.PrioritizationScoreBreakdownId)
                .Index(t => t.GrantStreamEligibilityQuestionId)
                .Index(t => t.PrioritizationScoreBreakdownId);
            
            CreateTable(
                "dbo.PrioritizationIndustryScores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NaicsCode = c.String(),
                        IndustryScore = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrioritizationRegions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RegionalScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrioritizationThresholds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IndustryThreshold = c.Int(nullable: false),
                        RegionalThreshold = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EmployeeCountThreshold = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.GrantApplications", "PrioritizationScore", c => c.Int(nullable: false));
            AddColumn("dbo.GrantApplications", "PrioritizationScoreBreakdownId", c => c.Int());
            AddColumn("dbo.GrantStreamEligibilityQuestions", "EligibilityPositiveAnswerPriorityScore", c => c.Int(nullable: false));
            CreateIndex("dbo.GrantApplications", "PrioritizationScoreBreakdownId");
            AddForeignKey("dbo.GrantApplications", "PrioritizationScoreBreakdownId", "dbo.PrioritizationScoreBreakdowns", "Id");

            PostDeployment();
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GrantApplications", "PrioritizationScoreBreakdownId", "dbo.PrioritizationScoreBreakdowns");
            DropForeignKey("dbo.PrioritizationScoreBreakdownAnswers", "PrioritizationScoreBreakdownId", "dbo.PrioritizationScoreBreakdowns");
            DropForeignKey("dbo.PrioritizationScoreBreakdownAnswers", "GrantStreamEligibilityQuestionId", "dbo.GrantStreamEligibilityQuestions");
            DropIndex("dbo.PrioritizationScoreBreakdownAnswers", new[] { "PrioritizationScoreBreakdownId" });
            DropIndex("dbo.PrioritizationScoreBreakdownAnswers", new[] { "GrantStreamEligibilityQuestionId" });
            DropIndex("dbo.GrantApplications", new[] { "PrioritizationScoreBreakdownId" });
            DropColumn("dbo.GrantStreamEligibilityQuestions", "EligibilityPositiveAnswerPriorityScore");
            DropColumn("dbo.GrantApplications", "PrioritizationScoreBreakdownId");
            DropColumn("dbo.GrantApplications", "PrioritizationScore");
            DropTable("dbo.PrioritizationThresholds");
            DropTable("dbo.PrioritizationRegions");
            DropTable("dbo.PrioritizationIndustryScores");
            DropTable("dbo.PrioritizationScoreBreakdownAnswers");
            DropTable("dbo.PrioritizationScoreBreakdowns");
        }
    }
}
