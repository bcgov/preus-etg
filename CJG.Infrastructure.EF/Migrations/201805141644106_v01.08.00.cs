namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.ComponentModel;
    using System.Data.Entity.Migrations;
    
    [Description("v01.08.00")]
    public partial class v010800 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropForeignKey("dbo.GrantStreams", "StreamId", "dbo.Streams");
            DropForeignKey("dbo.StreamFiscals", "FiscalYearId", "dbo.FiscalYears");
            DropForeignKey("dbo.StreamFiscals", "StreamId", "dbo.Streams");
            DropForeignKey("dbo.GrantStreams", "StreamCriteriaId", "dbo.StreamCriterias");
            DropForeignKey("dbo.GrantStreams", "StreamObjectiveId", "dbo.StreamObjectives");
            DropForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents");
            DropIndex("dbo.ParticipantForms", new[] { "ParticipantConsentId" });
            DropIndex("dbo.DocumentTemplates", "IX_DocumentTemplate");
            DropIndex("dbo.GrantStreams", "IX_GrantStream");
            DropIndex("dbo.GrantStreams", new[] { "StreamCriteriaId" });
            DropIndex("dbo.GrantStreams", new[] { "StreamObjectiveId" });
            DropIndex("dbo.StreamFiscals", new[] { "StreamId" });
            DropIndex("dbo.StreamFiscals", new[] { "FiscalYearId" });
            DropIndex("dbo.StreamCriterias", new[] { "Caption" });
            DropIndex("dbo.StreamObjectives", new[] { "Caption" });
            CreateTable(
                "dbo.AccountCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GLClientNumber = c.String(nullable: false, maxLength: 50),
                        GLRESP = c.String(nullable: false, maxLength: 20),
                        GLServiceLine = c.String(nullable: false, maxLength: 20),
                        GLSTOBNormal = c.String(nullable: false, maxLength: 20),
                        GLSTOBAccrual = c.String(nullable: false, maxLength: 20),
                        GLProjectCode = c.String(nullable: false, maxLength: 20),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GrantPrograms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountCodeId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 100),
                        Abbreviation = c.String(nullable: false, maxLength: 5),
                        Description = c.String(maxLength: 2000),
                        Message = c.String(maxLength: 1000),
                        ShowMessage = c.Boolean(nullable: false),
                        CanReportParticipants = c.Boolean(nullable: false),
                        CanReportSponsors = c.Boolean(nullable: false),
                        State = c.Int(nullable: false),
                        ApplicantDeclarationTemplateId = c.Int(nullable: false),
                        ApplicantCoverLetterTemplateId = c.Int(nullable: false),
                        ApplicantScheduleATemplateId = c.Int(nullable: false),
                        ApplicantScheduleBTemplateId = c.Int(nullable: false),
                        ParticipantConsentTemplateId = c.Int(nullable: false),
                        ExpenseAuthorityId = c.Int(),
                        RequestedBy = c.String(maxLength: 250),
                        ProgramPhone = c.String(maxLength: 50),
                        DocumentPrefix = c.String(maxLength: 5),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountCodes", t => t.AccountCodeId)
                .ForeignKey("dbo.DocumentTemplates", t => t.ApplicantCoverLetterTemplateId)
                .ForeignKey("dbo.DocumentTemplates", t => t.ApplicantDeclarationTemplateId)
                .ForeignKey("dbo.DocumentTemplates", t => t.ApplicantScheduleATemplateId)
                .ForeignKey("dbo.DocumentTemplates", t => t.ApplicantScheduleBTemplateId)
                .ForeignKey("dbo.InternalUsers", t => t.ExpenseAuthorityId)
                .ForeignKey("dbo.DocumentTemplates", t => t.ParticipantConsentTemplateId)
                .Index(t => new { t.AccountCodeId, t.Name, t.Abbreviation, t.State }, unique: true, name: "IX_GrantProgram")
                .Index(t => t.ApplicantDeclarationTemplateId)
                .Index(t => t.ApplicantCoverLetterTemplateId)
                .Index(t => t.ApplicantScheduleATemplateId)
                .Index(t => t.ApplicantScheduleBTemplateId)
                .Index(t => t.ParticipantConsentTemplateId)
                .Index(t => t.ExpenseAuthorityId);
            
            CreateTable(
                "dbo.GrantProgramNotifications",
                c => new
                    {
                        GrantProgramId = c.Int(nullable: false),
                        NotificationTypeId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.GrantProgramId, t.NotificationTypeId })
                .ForeignKey("dbo.GrantPrograms", t => t.GrantProgramId, cascadeDelete: true)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId, cascadeDelete: true)
                .Index(t => t.GrantProgramId)
                .Index(t => t.NotificationTypeId);
            
            CreateTable(
                "dbo.ReportRates",
                c => new
                    {
                        FiscalYearId = c.Int(nullable: false),
                        GrantProgramId = c.Int(nullable: false),
                        GrantStreamId = c.Int(nullable: false),
                        AgreementCancellationRate = c.Double(nullable: false),
                        AgreementSlippageRate = c.Double(nullable: false),
                        ClaimSlippageRate = c.Double(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.FiscalYearId, t.GrantProgramId, t.GrantStreamId })
                .ForeignKey("dbo.FiscalYears", t => t.FiscalYearId, cascadeDelete: true)
                .ForeignKey("dbo.GrantPrograms", t => t.GrantProgramId, cascadeDelete: true)
                .Index(t => t.FiscalYearId)
                .Index(t => t.GrantProgramId);
            
            AddColumn("dbo.NotificationTypes", "Description", c => c.String(maxLength: 500));
            AddColumn("dbo.NotificationTypes", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Organizations", "BusinessType", c => c.String(maxLength: 250));
            AddColumn("dbo.Organizations", "StatementOfRegistrationNumber", c => c.String(maxLength: 250));
            AddColumn("dbo.Organizations", "IncorporationNumber", c => c.String(maxLength: 250));
            AddColumn("dbo.Organizations", "JurisdictionOfIncorporation", c => c.String(maxLength: 250));
            AddColumn("dbo.DocumentTemplates", "Description", c => c.String(maxLength: 500));
            AddColumn("dbo.GrantStreams", "GrantProgramId", c => c.Int());
            AddColumn("dbo.GrantStreams", "AccountCodeId", c => c.Int());
            AddColumn("dbo.GrantStreams", "Name", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.GrantStreams", "Criteria", c => c.String(maxLength: 2500));
            AddColumn("dbo.GrantStreams", "Objective", c => c.String(maxLength: 2500));
            AddColumn("dbo.GrantStreams", "MaxReimbursementAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.GrantStreams", "ReimbursementRate", c => c.Double(nullable: false));
            AddColumn("dbo.GrantStreams", "DefaultDeniedRate", c => c.Double(nullable: false));
            AddColumn("dbo.GrantStreams", "DefaultWithdrawnRate", c => c.Double(nullable: false));
            AddColumn("dbo.GrantStreams", "DefaultReductionRate", c => c.Double(nullable: false));
            AddColumn("dbo.GrantStreams", "DefaultSlippageRate", c => c.Double(nullable: false));
            AddColumn("dbo.GrantStreams", "DefaultCancellationRate", c => c.Double(nullable: false));
            AddColumn("dbo.GrantStreams", "IncludeDeliveryPartner", c => c.Boolean(nullable: false));
            AddColumn("dbo.TrainingPeriods", "ShortName", c => c.String(maxLength: 20));
            AlterColumn("dbo.ParticipantForms", "ParticipantConsentId", c => c.Int());
            AlterColumn("dbo.DocumentTemplates", "Title", c => c.String(nullable: false, maxLength: 250));
            CreateIndex("dbo.DocumentTemplates", new[] { "DocumentType", "IsActive", "Title" }, name: "IX_DocumentTemplate");
            CreateIndex("dbo.InternalUsers", new[] { "LastName", "FirstName" }, name: "IX_InternalUser_Name");
            CreateIndex("dbo.NotificationTypes", "IsActive", name: "IX_NotificationType");
            CreateIndex("dbo.ParticipantForms", "ParticipantConsentId");
            CreateIndex("dbo.GrantStreams", new[] { "GrantProgramId", "AccountCodeId", "IsActive" }, name: "IX_GrantStream");
            AddForeignKey("dbo.GrantStreams", "AccountCodeId", "dbo.AccountCodes", "Id");
            AddForeignKey("dbo.GrantStreams", "GrantProgramId", "dbo.GrantPrograms", "Id");
            AddForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents", "Id");
            DropColumn("dbo.GrantStreams", "StreamId");
            DropColumn("dbo.GrantStreams", "StreamCriteriaId");
            DropColumn("dbo.GrantStreams", "StreamObjectiveId");
            DropTable("dbo.Streams");
            DropTable("dbo.StreamFiscals");
            DropTable("dbo.StreamCriterias");
            DropTable("dbo.StreamObjectives");

            PostDeployment();
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StreamObjectives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 2000),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StreamCriterias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 2000),
                        Caption = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StreamFiscals",
                c => new
                    {
                        StreamId = c.Int(nullable: false),
                        FiscalYearId = c.Int(nullable: false),
                        ClaimSlippageRate = c.Double(nullable: false),
                        AgreementCancellationRate = c.Double(nullable: false),
                        AgreementSlippageRate = c.Double(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.StreamId, t.FiscalYearId });
            
            CreateTable(
                "dbo.Streams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        MaxReimbursementAmt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReimbursementRate = c.Double(nullable: false),
                        DefaultDeniedRate = c.Double(nullable: false),
                        DefaultWithdrawnRate = c.Double(nullable: false),
                        DefaultReductionRate = c.Double(nullable: false),
                        DefaultSlippageRate = c.Double(nullable: false),
                        DefaultCancellationRate = c.Double(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.GrantStreams", "StreamObjectiveId", c => c.Int(nullable: false));
            AddColumn("dbo.GrantStreams", "StreamCriteriaId", c => c.Int(nullable: false));
            AddColumn("dbo.GrantStreams", "StreamId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents");
            DropForeignKey("dbo.GrantPrograms", "ParticipantConsentTemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.GrantPrograms", "ExpenseAuthorityId", "dbo.InternalUsers");
            DropForeignKey("dbo.ReportRates", "GrantProgramId", "dbo.GrantPrograms");
            DropForeignKey("dbo.ReportRates", "FiscalYearId", "dbo.FiscalYears");
            DropForeignKey("dbo.GrantStreams", "GrantProgramId", "dbo.GrantPrograms");
            DropForeignKey("dbo.GrantStreams", "AccountCodeId", "dbo.AccountCodes");
            DropForeignKey("dbo.GrantProgramNotifications", "NotificationTypeId", "dbo.NotificationTypes");
            DropForeignKey("dbo.GrantProgramNotifications", "GrantProgramId", "dbo.GrantPrograms");
            DropForeignKey("dbo.GrantPrograms", "ApplicantScheduleBTemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.GrantPrograms", "ApplicantScheduleATemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.GrantPrograms", "ApplicantDeclarationTemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.GrantPrograms", "ApplicantCoverLetterTemplateId", "dbo.DocumentTemplates");
            DropForeignKey("dbo.GrantPrograms", "AccountCodeId", "dbo.AccountCodes");
            DropIndex("dbo.ReportRates", new[] { "GrantProgramId" });
            DropIndex("dbo.ReportRates", new[] { "FiscalYearId" });
            DropIndex("dbo.GrantStreams", "IX_GrantStream");
            DropIndex("dbo.ParticipantForms", new[] { "ParticipantConsentId" });
            DropIndex("dbo.GrantProgramNotifications", new[] { "NotificationTypeId" });
            DropIndex("dbo.GrantProgramNotifications", new[] { "GrantProgramId" });
            DropIndex("dbo.NotificationTypes", "IX_NotificationType");
            DropIndex("dbo.InternalUsers", "IX_InternalUser_Name");
            DropIndex("dbo.DocumentTemplates", "IX_DocumentTemplate");
            DropIndex("dbo.GrantPrograms", new[] { "ExpenseAuthorityId" });
            DropIndex("dbo.GrantPrograms", new[] { "ParticipantConsentTemplateId" });
            DropIndex("dbo.GrantPrograms", new[] { "ApplicantScheduleBTemplateId" });
            DropIndex("dbo.GrantPrograms", new[] { "ApplicantScheduleATemplateId" });
            DropIndex("dbo.GrantPrograms", new[] { "ApplicantCoverLetterTemplateId" });
            DropIndex("dbo.GrantPrograms", new[] { "ApplicantDeclarationTemplateId" });
            DropIndex("dbo.GrantPrograms", "IX_GrantProgram");
            AlterColumn("dbo.DocumentTemplates", "Title", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.ParticipantForms", "ParticipantConsentId", c => c.Int(nullable: false));
            DropColumn("dbo.TrainingPeriods", "ShortName");
            DropColumn("dbo.GrantStreams", "IncludeDeliveryPartner");
            DropColumn("dbo.GrantStreams", "DefaultCancellationRate");
            DropColumn("dbo.GrantStreams", "DefaultSlippageRate");
            DropColumn("dbo.GrantStreams", "DefaultReductionRate");
            DropColumn("dbo.GrantStreams", "DefaultWithdrawnRate");
            DropColumn("dbo.GrantStreams", "DefaultDeniedRate");
            DropColumn("dbo.GrantStreams", "ReimbursementRate");
            DropColumn("dbo.GrantStreams", "MaxReimbursementAmt");
            DropColumn("dbo.GrantStreams", "Objective");
            DropColumn("dbo.GrantStreams", "Criteria");
            DropColumn("dbo.GrantStreams", "Name");
            DropColumn("dbo.GrantStreams", "AccountCodeId");
            DropColumn("dbo.GrantStreams", "GrantProgramId");
            DropColumn("dbo.DocumentTemplates", "Description");
            DropColumn("dbo.Organizations", "JurisdictionOfIncorporation");
            DropColumn("dbo.Organizations", "IncorporationNumber");
            DropColumn("dbo.Organizations", "StatementOfRegistrationNumber");
            DropColumn("dbo.Organizations", "BusinessType");
            DropColumn("dbo.NotificationTypes", "IsActive");
            DropColumn("dbo.NotificationTypes", "Description");
            DropTable("dbo.ReportRates");
            DropTable("dbo.GrantProgramNotifications");
            DropTable("dbo.GrantPrograms");
            DropTable("dbo.AccountCodes");
            CreateIndex("dbo.StreamObjectives", "Caption", unique: true);
            CreateIndex("dbo.StreamCriterias", "Caption", unique: true);
            CreateIndex("dbo.StreamFiscals", "FiscalYearId");
            CreateIndex("dbo.StreamFiscals", "StreamId");
            CreateIndex("dbo.GrantStreams", "StreamObjectiveId");
            CreateIndex("dbo.GrantStreams", "StreamCriteriaId");
            CreateIndex("dbo.GrantStreams", new[] { "IsActive", "StreamId" }, name: "IX_GrantStream");
            CreateIndex("dbo.DocumentTemplates", new[] { "DocumentType", "IsActive" }, name: "IX_DocumentTemplate");
            CreateIndex("dbo.ParticipantForms", "ParticipantConsentId");
            AddForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GrantStreams", "StreamObjectiveId", "dbo.StreamObjectives", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GrantStreams", "StreamCriteriaId", "dbo.StreamCriterias", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StreamFiscals", "StreamId", "dbo.Streams", "Id", cascadeDelete: true);
            AddForeignKey("dbo.StreamFiscals", "FiscalYearId", "dbo.FiscalYears", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GrantStreams", "StreamId", "dbo.Streams", "Id", cascadeDelete: true);
        }
    }
}
