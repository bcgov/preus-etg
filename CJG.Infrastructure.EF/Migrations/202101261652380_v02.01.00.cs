namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v02.01.00")]
	public partial class v020100 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DenialReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Caption = c.String(nullable: false, maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                        RowSequence = c.Int(nullable: false),
                        GrantProgramId = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrantPrograms", t => t.GrantProgramId, cascadeDelete: true)
                .Index(t => t.Caption)
                .Index(t => t.GrantProgramId);

            CreateTable(
                "dbo.GrantApplicationDenialReasons",
                c => new
                    {
                        GrantApplicationId = c.Int(nullable: false),
                        DenialReasonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GrantApplicationId, t.DenialReasonId })
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .ForeignKey("dbo.DenialReasons", t => t.DenialReasonId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.DenialReasonId);

            AddColumn("dbo.GrantApplications", "HasRequestedAdditionalFunding", c => c.Boolean());
            AddColumn("dbo.GrantApplications", "DescriptionOfFundingRequested", c => c.String());
            AddColumn("dbo.GrantApplications", "BusinessCase", c => c.String());
            AddColumn("dbo.GrantApplications", "BusinessCaseDocumentId", c => c.Int());
            AddColumn("dbo.TrainingProviders", "TrainingProviderAddressId", c => c.Int());
            AddColumn("dbo.TrainingProviderTypes", "ProofOfInstructorQualifications", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingProviderTypes", "CourseOutline", c => c.Int(nullable: false));
            AddColumn("dbo.GrantStreams", "BusinessCaseIsEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "BusinessCaseRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.GrantStreams", "BusinessCaseInternalHeader", c => c.String(maxLength: 200));
            AddColumn("dbo.GrantStreams", "BusinessCaseExternalHeader", c => c.String(maxLength: 200));
            AddColumn("dbo.GrantStreams", "BusinessCaseUserGuidance", c => c.String(maxLength: 2500));
            AddColumn("dbo.GrantStreams", "BusinessCaseTemplateURL", c => c.String(maxLength: 800));
            AlterColumn("dbo.ParticipantForms", "PersonAboriginal", c => c.Int());
            AlterColumn("dbo.ParticipantForms", "VisibleMinority", c => c.Int());
            AlterColumn("dbo.ParticipantForms", "CanadaImmigrant", c => c.Boolean());
            AlterColumn("dbo.ParticipantForms", "CanadaRefugee", c => c.Boolean());
            CreateIndex("dbo.GrantApplications", "BusinessCaseDocumentId");
            CreateIndex("dbo.TrainingProviders", "TrainingProviderAddressId");
            AddForeignKey("dbo.GrantApplications", "BusinessCaseDocumentId", "dbo.Attachments", "Id");
            AddForeignKey("dbo.TrainingProviders", "TrainingProviderAddressId", "dbo.ApplicationAddresses", "Id");
			AlterColumn("dbo.TrainingProviders", "TrainingAddressId", c => c.Int(nullable: true));

			PostDeployment();
		}

        public override void Down()
        {
            DropForeignKey("dbo.GrantApplicationDenialReasons", "DenialReasonId", "dbo.DenialReasons");
            DropForeignKey("dbo.GrantApplicationDenialReasons", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.TrainingProviders", "TrainingProviderAddressId", "dbo.ApplicationAddresses");
            DropForeignKey("dbo.GrantApplications", "BusinessCaseDocumentId", "dbo.Attachments");
            DropForeignKey("dbo.DenialReasons", "GrantProgramId", "dbo.GrantPrograms");
            DropIndex("dbo.GrantApplicationDenialReasons", new[] { "DenialReasonId" });
            DropIndex("dbo.GrantApplicationDenialReasons", new[] { "GrantApplicationId" });
            DropIndex("dbo.TrainingProviders", new[] { "TrainingProviderAddressId" });
            DropIndex("dbo.GrantApplications", new[] { "BusinessCaseDocumentId" });
            DropIndex("dbo.DenialReasons", new[] { "GrantProgramId" });
            DropIndex("dbo.DenialReasons", new[] { "Caption" });
            AlterColumn("dbo.ParticipantForms", "CanadaRefugee", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ParticipantForms", "CanadaImmigrant", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ParticipantForms", "VisibleMinority", c => c.Int(nullable: false));
            AlterColumn("dbo.ParticipantForms", "PersonAboriginal", c => c.Int(nullable: false));
            DropColumn("dbo.GrantStreams", "BusinessCaseTemplateURL");
            DropColumn("dbo.GrantStreams", "BusinessCaseUserGuidance");
            DropColumn("dbo.GrantStreams", "BusinessCaseExternalHeader");
            DropColumn("dbo.GrantStreams", "BusinessCaseInternalHeader");
            DropColumn("dbo.GrantStreams", "BusinessCaseRequired");
            DropColumn("dbo.GrantStreams", "BusinessCaseIsEnabled");
            DropColumn("dbo.TrainingProviderTypes", "CourseOutline");
            DropColumn("dbo.TrainingProviderTypes", "ProofOfInstructorQualifications");
            DropColumn("dbo.TrainingProviders", "TrainingProviderAddressId");
            DropColumn("dbo.GrantApplications", "BusinessCaseDocumentId");
            DropColumn("dbo.GrantApplications", "BusinessCase");
            DropColumn("dbo.GrantApplications", "DescriptionOfFundingRequested");
            DropColumn("dbo.GrantApplications", "HasRequestedAdditionalFunding");
            DropTable("dbo.GrantApplicationDenialReasons");
            DropTable("dbo.DenialReasons");
        }
    }
}
