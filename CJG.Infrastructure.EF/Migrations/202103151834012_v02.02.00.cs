namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v02.02.00")]
	public partial class v020200 : ExtendedDbMigration
	{
        public override void Up()
        {
            CreateTable(
                "dbo.ParticipantCompletionReportMultAnswers",
                c => new
                    {
                        ParticipantFormId = c.Int(nullable: false),
                        GrantApplicationId = c.Int(nullable: false),
                        ParticipantCompletionAnswerId = c.Int(nullable: false),
                        CompletionReportOptionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParticipantFormId, t.GrantApplicationId, t.ParticipantCompletionAnswerId, t.CompletionReportOptionId })
                .ForeignKey("dbo.ParticipantCompletionReportAnswers", t => new { t.ParticipantFormId, t.GrantApplicationId, t.ParticipantCompletionAnswerId })
                .ForeignKey("dbo.CompletionReportOptions", t => t.CompletionReportOptionId)
                .Index(t => new { t.ParticipantFormId, t.GrantApplicationId, t.ParticipantCompletionAnswerId })
                .Index(t => t.CompletionReportOptionId);

            AddColumn("dbo.ParticipantCompletionReportAnswers", "CommunityId", c => c.Int());
            AddColumn("dbo.CompletionReportOptions", "NextQuestion", c => c.Int(nullable: false));
            AddColumn("dbo.CompletionReportQuestions", "DisplayOnlyIfGoto", c => c.Boolean(nullable: false));
            AddColumn("dbo.CompletionReportQuestions", "NextQuestion", c => c.Int());
            AlterColumn("dbo.EmployerCompletionReportAnswers", "OtherAnswer", c => c.String(maxLength: 2000));
            CreateIndex("dbo.ParticipantCompletionReportAnswers", "CommunityId");
            AddForeignKey("dbo.ParticipantCompletionReportAnswers", "CommunityId", "dbo.Communities", "Id");

			PostDeployment();
		}

        public override void Down()
        {
			DropForeignKey("dbo.ParticipantCompletionReportMultAnswers", "CompletionReportOptionId", "dbo.CompletionReportOptions");
            DropForeignKey("dbo.ParticipantCompletionReportMultAnswers", new[] { "ParticipantFormId", "GrantApplicationId", "ParticipantCompletionAnswerId" }, "dbo.ParticipantCompletionReportAnswers");
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "CommunityId", "dbo.Communities");
            DropIndex("dbo.ParticipantCompletionReportMultAnswers", new[] { "CompletionReportOptionId" });
            DropIndex("dbo.ParticipantCompletionReportMultAnswers", new[] { "ParticipantFormId", "GrantApplicationId", "ParticipantCompletionAnswerId" });
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "CommunityId" });
            AlterColumn("dbo.EmployerCompletionReportAnswers", "OtherAnswer", c => c.String(maxLength: 500));
            DropColumn("dbo.CompletionReportQuestions", "NextQuestion");
            DropColumn("dbo.CompletionReportQuestions", "DisplayOnlyIfGoto");
            DropColumn("dbo.CompletionReportOptions", "NextQuestion");
            DropColumn("dbo.ParticipantCompletionReportAnswers", "CommunityId");
            DropTable("dbo.ParticipantCompletionReportMultAnswers");
        }
    }
}
