using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{

    [Description("v01.10.00")]
    public partial class v011000 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropForeignKey("dbo.TrainingPrograms", "DeliveryPartnerId", "dbo.DeliveryPartners");
            DropForeignKey("dbo.TrainingProgramDeliveryPartnerServices", "TrainingProgramId", "dbo.TrainingPrograms");
            DropForeignKey("dbo.TrainingProgramDeliveryPartnerServices", "DeliveryPartnerServiceId", "dbo.DeliveryPartnerServices");
            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            DropIndex("dbo.TrainingPrograms", "IX_TrainingPrograms");
            DropIndex("dbo.TrainingProgramDeliveryPartnerServices", new[] { "TrainingProgramId" });
            DropIndex("dbo.TrainingProgramDeliveryPartnerServices", new[] { "DeliveryPartnerServiceId" });
            CreateTable(
                "dbo.GrantApplicationDeliveryPartnerServices",
                c => new
                    {
                        GrantApplicationId = c.Int(nullable: false),
                        DeliveryPartnerServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GrantApplicationId, t.DeliveryPartnerServiceId })
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .ForeignKey("dbo.DeliveryPartnerServices", t => t.DeliveryPartnerServiceId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.DeliveryPartnerServiceId);
            
            AddColumn("dbo.GrantApplications", "UsedDeliveryPartner", c => c.Boolean());
            AddColumn("dbo.GrantApplications", "DeliveryPartnerId", c => c.Int());
            AddColumn("dbo.EligibleCostBreakdowns", "IsEligible", c => c.Boolean(nullable: false));
            AddColumn("dbo.EligibleCostBreakdowns", "AddedByAssessor", c => c.Boolean(nullable: false));
            AddColumn("dbo.ParticipantForms", "IsExcludedFromClaim", c => c.Boolean(nullable: false));
            AddColumn("dbo.ParticipantForms", "ParticipantConsentAttachmentId", c => c.Int());
            CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "StartDate", "EndDate", "InvitationKey", "InvitationExpiresOn", "DeliveryPartnerId" }, name: "IX_GrantApplication");
            CreateIndex("dbo.TrainingPrograms", new[] { "TrainingProgramState", "DateAdded", "GrantApplicationId", "StartDate", "EndDate", "InDemandOccupationId", "SkillLevelId", "SkillFocusId", "ExpectedQualificationId", "TrainingLevelId" }, name: "IX_TrainingPrograms");
            CreateIndex("dbo.ParticipantForms", "ParticipantConsentAttachmentId");
            AddForeignKey("dbo.ParticipantForms", "ParticipantConsentAttachmentId", "dbo.Attachments", "Id");
            AddForeignKey("dbo.GrantApplications", "DeliveryPartnerId", "dbo.DeliveryPartners", "Id");
            DropColumn("dbo.TrainingPrograms", "UsedDeliveryPartner");
            DropColumn("dbo.TrainingPrograms", "DeliveryPartnerId");
            DropTable("dbo.TrainingProgramDeliveryPartnerServices");

            PostDeployment();
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TrainingProgramDeliveryPartnerServices",
                c => new
                    {
                        TrainingProgramId = c.Int(nullable: false),
                        DeliveryPartnerServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TrainingProgramId, t.DeliveryPartnerServiceId });
            
            AddColumn("dbo.TrainingPrograms", "DeliveryPartnerId", c => c.Int());
            AddColumn("dbo.TrainingPrograms", "UsedDeliveryPartner", c => c.Boolean());
            DropForeignKey("dbo.GrantApplicationDeliveryPartnerServices", "DeliveryPartnerServiceId", "dbo.DeliveryPartnerServices");
            DropForeignKey("dbo.GrantApplicationDeliveryPartnerServices", "GrantApplicationId", "dbo.GrantApplications");
            DropForeignKey("dbo.GrantApplications", "DeliveryPartnerId", "dbo.DeliveryPartners");
            DropForeignKey("dbo.ParticipantForms", "ParticipantConsentAttachmentId", "dbo.Attachments");
            DropIndex("dbo.GrantApplicationDeliveryPartnerServices", new[] { "DeliveryPartnerServiceId" });
            DropIndex("dbo.GrantApplicationDeliveryPartnerServices", new[] { "GrantApplicationId" });
            DropIndex("dbo.ParticipantForms", new[] { "ParticipantConsentAttachmentId" });
            DropIndex("dbo.TrainingPrograms", "IX_TrainingPrograms");
            DropIndex("dbo.GrantApplications", "IX_GrantApplication");
            DropColumn("dbo.ParticipantForms", "ParticipantConsentAttachmentId");
            DropColumn("dbo.ParticipantForms", "IsExcludedFromClaim");
            DropColumn("dbo.EligibleCostBreakdowns", "AddedByAssessor");
            DropColumn("dbo.EligibleCostBreakdowns", "IsEligible");
            DropColumn("dbo.GrantApplications", "DeliveryPartnerId");
            DropColumn("dbo.GrantApplications", "UsedDeliveryPartner");
            DropTable("dbo.GrantApplicationDeliveryPartnerServices");
            CreateIndex("dbo.TrainingProgramDeliveryPartnerServices", "DeliveryPartnerServiceId");
            CreateIndex("dbo.TrainingProgramDeliveryPartnerServices", "TrainingProgramId");
            CreateIndex("dbo.TrainingPrograms", new[] { "TrainingProgramState", "DateAdded", "GrantApplicationId", "StartDate", "EndDate", "InDemandOccupationId", "SkillLevelId", "SkillFocusId", "ExpectedQualificationId", "TrainingLevelId", "DeliveryPartnerId" }, name: "IX_TrainingPrograms");
            CreateIndex("dbo.GrantApplications", new[] { "ApplicationStateInternal", "ApplicationStateExternal", "GrantOpeningId", "AssessorId", "OrganizationLegalName", "ApplicationTypeId", "StartDate", "EndDate", "InvitationKey", "InvitationExpiresOn" }, name: "IX_GrantApplication");
            AddForeignKey("dbo.TrainingProgramDeliveryPartnerServices", "DeliveryPartnerServiceId", "dbo.DeliveryPartnerServices", "Id");
            AddForeignKey("dbo.TrainingProgramDeliveryPartnerServices", "TrainingProgramId", "dbo.TrainingPrograms", "Id");
            AddForeignKey("dbo.TrainingPrograms", "DeliveryPartnerId", "dbo.DeliveryPartners", "Id");
        }
    }
}
