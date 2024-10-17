namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.ComponentModel;
    using System.Data.Entity.Migrations;

    [Description("v01.07.00")]
    public partial class v010700 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            CreateTable(
                "dbo.PaymentRequests",
                c => new
                {
                    PaymentRequestBatchId = c.Int(nullable: false),
                    TrainingProgramId = c.Int(nullable: false),
                    ClaimId = c.Int(),
                    ClaimVersion = c.Int(),
                    PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    PaymentType = c.Int(nullable: false),
                    DocumentNumber = c.String(nullable: false, maxLength: 100),
                    ReconcileToCAS = c.Boolean(nullable: false),
                    RecipientBusinessNumber = c.String(maxLength: 100),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => new { t.PaymentRequestBatchId, t.TrainingProgramId })
                .ForeignKey("dbo.Claims", t => new { t.ClaimId, t.ClaimVersion })
                .ForeignKey("dbo.PaymentRequestBatches", t => t.PaymentRequestBatchId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingPrograms", t => t.TrainingProgramId, cascadeDelete: true)
                .Index(t => t.PaymentRequestBatchId)
                .Index(t => t.TrainingProgramId)
                .Index(t => new { t.ClaimId, t.ClaimVersion });

            CreateTable(
                "dbo.PaymentRequestBatches",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    BatchNumber = c.String(nullable: false, maxLength: 4),
                    BatchType = c.Int(nullable: false),
                    IssuedDate = c.DateTime(),
                    IssuedById = c.Int(nullable: false),
                    Status = c.Int(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InternalUsers", t => t.IssuedById, cascadeDelete: true)
                .Index(t => t.IssuedById);

            CreateTable(
                "dbo.CompletionReportOptions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    QuestionId = c.Int(nullable: false),
                    Answer = c.String(nullable: false, maxLength: 500),
                    Level = c.Int(nullable: false),
                    TriggersNextLevel = c.Boolean(nullable: false),
                    Sequence = c.Int(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    DisplayOther = c.Boolean(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompletionReportQuestions", t => t.QuestionId)
                .Index(t => t.QuestionId);

            CreateTable(
                "dbo.CompletionReportQuestions",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CompletionReportId = c.Int(nullable: false),
                    Question = c.String(nullable: false, maxLength: 500),
                    Description = c.String(maxLength: 500),
                    Audience = c.Int(nullable: false),
                    GroupId = c.Int(nullable: false),
                    Sequence = c.Int(nullable: false),
                    IsRequired = c.Boolean(nullable: false),
                    IsActive = c.Boolean(nullable: false),
                    QuestionType = c.Int(nullable: false),
                    DefaultText = c.String(maxLength: 500),
                    DefaultAnswerId = c.Int(),
                    ContinueIfAnswerId = c.Int(),
                    StopIfAnswerId = c.Int(),
                    AnswerTableHeadings = c.String(),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompletionReports", t => t.CompletionReportId, cascadeDelete: true)
                .ForeignKey("dbo.CompletionReportOptions", t => t.ContinueIfAnswerId)
                .ForeignKey("dbo.CompletionReportOptions", t => t.DefaultAnswerId)
                .ForeignKey("dbo.CompletionReportGroups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.CompletionReportOptions", t => t.StopIfAnswerId)
                .Index(t => t.CompletionReportId)
                .Index(t => t.GroupId)
                .Index(t => t.DefaultAnswerId)
                .Index(t => t.ContinueIfAnswerId)
                .Index(t => t.StopIfAnswerId);

            CreateTable(
                "dbo.CompletionReports",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Caption = c.String(nullable: false, maxLength: 250),
                    Description = c.String(maxLength: 500),
                    IsActive = c.Boolean(nullable: false),
                    EffectiveDate = c.DateTime(nullable: false),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CompletionReportGroups",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Title = c.String(),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.EmployerCompletionReportAnswers",
                c => new
                {
                    EmployerEnrollmentId = c.Int(nullable: false),
                    QuestionId = c.Int(nullable: false),
                    AnswerId = c.Int(),
                    OtherAnswer = c.String(maxLength: 500),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => new { t.EmployerEnrollmentId, t.QuestionId })
                .ForeignKey("dbo.CompletionReportOptions", t => t.AnswerId)
                .ForeignKey("dbo.EmployerEnrollments", t => t.EmployerEnrollmentId, cascadeDelete: true)
                .ForeignKey("dbo.CompletionReportQuestions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.EmployerEnrollmentId)
                .Index(t => t.QuestionId)
                .Index(t => t.AnswerId);

            CreateTable(
                "dbo.ParticipantCompletionReportAnswers",
                c => new
                {
                    ParticipantEnrollmentId = c.Int(nullable: false),
                    QuestionId = c.Int(nullable: false),
                    AnswerId = c.Int(),
                    OtherAnswer = c.String(maxLength: 500),
                    DateAdded = c.DateTime(nullable: false),
                    DateUpdated = c.DateTime(),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => new { t.ParticipantEnrollmentId, t.QuestionId })
                .ForeignKey("dbo.CompletionReportOptions", t => t.AnswerId)
                .ForeignKey("dbo.ParticipantEnrollments", t => t.ParticipantEnrollmentId, cascadeDelete: true)
                .ForeignKey("dbo.CompletionReportQuestions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.ParticipantEnrollmentId)
                .Index(t => t.QuestionId)
                .Index(t => t.AnswerId);

            AddColumn("dbo.TrainingPrograms", "HoldPaymentRequests", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "BusinessNumber", c => c.String(maxLength: 250));
            AddColumn("dbo.Organizations", "BusinessNumberVerified", c => c.Boolean());

            PostDeployment();
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "QuestionId", "dbo.CompletionReportQuestions");
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "ParticipantEnrollmentId", "dbo.ParticipantEnrollments");
            DropForeignKey("dbo.ParticipantCompletionReportAnswers", "AnswerId", "dbo.CompletionReportOptions");
            DropForeignKey("dbo.EmployerCompletionReportAnswers", "QuestionId", "dbo.CompletionReportQuestions");
            DropForeignKey("dbo.EmployerCompletionReportAnswers", "EmployerEnrollmentId", "dbo.EmployerEnrollments");
            DropForeignKey("dbo.EmployerCompletionReportAnswers", "AnswerId", "dbo.CompletionReportOptions");
            DropForeignKey("dbo.CompletionReportOptions", "QuestionId", "dbo.CompletionReportQuestions");
            DropForeignKey("dbo.CompletionReportQuestions", "StopIfAnswerId", "dbo.CompletionReportOptions");
            DropForeignKey("dbo.CompletionReportQuestions", "GroupId", "dbo.CompletionReportGroups");
            DropForeignKey("dbo.CompletionReportQuestions", "DefaultAnswerId", "dbo.CompletionReportOptions");
            DropForeignKey("dbo.CompletionReportQuestions", "ContinueIfAnswerId", "dbo.CompletionReportOptions");
            DropForeignKey("dbo.CompletionReportQuestions", "CompletionReportId", "dbo.CompletionReports");
            DropForeignKey("dbo.PaymentRequests", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.PaymentRequests", "PaymentRequestBatchId", "dbo.PaymentRequestBatches");
            DropForeignKey("dbo.PaymentRequestBatches", "IssuedById", "dbo.InternalUsers");
            DropForeignKey("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" }, "dbo.Claims");
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "AnswerId" });
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "QuestionId" });
            DropIndex("dbo.ParticipantCompletionReportAnswers", new[] { "ParticipantEnrollmentId" });
            DropIndex("dbo.EmployerCompletionReportAnswers", new[] { "AnswerId" });
            DropIndex("dbo.EmployerCompletionReportAnswers", new[] { "QuestionId" });
            DropIndex("dbo.EmployerCompletionReportAnswers", new[] { "EmployerEnrollmentId" });
            DropIndex("dbo.CompletionReportQuestions", new[] { "StopIfAnswerId" });
            DropIndex("dbo.CompletionReportQuestions", new[] { "ContinueIfAnswerId" });
            DropIndex("dbo.CompletionReportQuestions", new[] { "DefaultAnswerId" });
            DropIndex("dbo.CompletionReportQuestions", new[] { "GroupId" });
            DropIndex("dbo.CompletionReportQuestions", new[] { "CompletionReportId" });
            DropIndex("dbo.CompletionReportOptions", new[] { "QuestionId" });
            DropIndex("dbo.PaymentRequestBatches", new[] { "IssuedById" });
            DropIndex("dbo.PaymentRequests", new[] { "ClaimId", "ClaimVersion" });
            DropIndex("dbo.PaymentRequests", new[] { "TrainingProgramId" });
            DropIndex("dbo.PaymentRequests", new[] { "PaymentRequestBatchId" });
            DropColumn("dbo.Organizations", "BusinessNumberVerified");
            DropColumn("dbo.Organizations", "BusinessNumber");
            DropColumn("dbo.TrainingPrograms", "HoldPaymentRequests");
            DropTable("dbo.ParticipantCompletionReportAnswers");
            DropTable("dbo.EmployerCompletionReportAnswers");
            DropTable("dbo.CompletionReportGroups");
            DropTable("dbo.CompletionReports");
            DropTable("dbo.CompletionReportQuestions");
            DropTable("dbo.CompletionReportOptions");
            DropTable("dbo.PaymentRequestBatches");
            DropTable("dbo.PaymentRequests");
        }
    }
}
