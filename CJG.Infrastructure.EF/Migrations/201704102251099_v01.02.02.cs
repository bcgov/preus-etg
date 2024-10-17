namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.02.02")]
    public partial class v010202 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            CreateTable(
                "dbo.ClaimStates",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000),
                    Caption = c.String(maxLength: 250),
                    IsActive = c.Boolean(nullable: false, defaultValue: true),
                    RowSequence = c.Int(nullable: false, defaultValue: 0),
                    DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                    DateUpdated = c.DateTime(storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);

            CreateTable(
                "dbo.GrantApplicationExternalStates",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000),
                    Caption = c.String(maxLength: 250),
                    IsActive = c.Boolean(nullable: false, defaultValue: true),
                    RowSequence = c.Int(nullable: false, defaultValue: 0),
                    DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                    DateUpdated = c.DateTime(storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);

            CreateTable(
                "dbo.GrantApplicationInternalStates",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000),
                    Caption = c.String(maxLength: 250),
                    IsActive = c.Boolean(nullable: false, defaultValue: true),
                    RowSequence = c.Int(nullable: false, defaultValue: 0),
                    DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                    DateUpdated = c.DateTime(storeType: "datetime2"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Caption, unique: true);

            AddColumn("dbo.GrantApplications", "OrganizationNumberOfEmployeesInBC", c => c.Int());
            AddColumn("dbo.ParticipantEnrollments", "ReportedDate", c => c.DateTime(storeType: "datetime2"));

            DropForeignKey("dbo.TrainingPrograms", "TrainingProviderId", "dbo.TrainingProviders");
            DropForeignKey("dbo.TrainingPrograms", "RequestedTrainingProviderId", "dbo.TrainingProviders");
            DropIndex("dbo.TrainingPrograms", new[] { "TrainingProviderId" });
            DropIndex("dbo.TrainingPrograms", new[] { "RequestedTrainingProviderId" });
            DropIndex("dbo.TrainingPrograms", new[] { "GrantApplicationId" });
            DropIndex("dbo.TrainingPrograms", new[] { "InDemandOccupationId" });
            DropIndex("dbo.TrainingPrograms", new[] { "SkillLevelId" });
            DropIndex("dbo.TrainingPrograms", new[] { "SkillFocusId" });
            DropIndex("dbo.TrainingPrograms", new[] { "ExpectedQualificationId" });
            DropIndex("dbo.TrainingPrograms", new[] { "TrainingLevelId" });
            DropIndex("dbo.TrainingPrograms", new[] { "DeliveryPartnerId" });
            DropIndex("dbo.TrainingProviders", new[] { "GrantApplicationId" });
            DropIndex("dbo.TrainingProviders", new[] { "TrainingProviderTypeId" });
            AddColumn("dbo.TrainingProviders", "TrainingProgramId", c => c.Int());

            DropColumn("dbo.TrainingPrograms", "TrainingProviderId");
            DropColumn("dbo.TrainingPrograms", "RequestedTrainingProviderId");

            CreateIndex("dbo.TrainingPrograms", new[] { "TrainingProgramState", "TrainingCostState", "DateAdded", "GrantApplicationId", "StartDate", "EndDate", "InDemandOccupationId", "SkillLevelId", "SkillFocusId", "ExpectedQualificationId", "TrainingLevelId", "DeliveryPartnerId" }, name: "IX_TrainingPrograms");
            CreateIndex("dbo.TrainingProviders", new[] { "TrainingProviderState", "Name", "GrantApplicationId", "TrainingProgramId", "TrainingProviderTypeId", "TrainingOutsideBC", "DateAdded", "ContactLastName", "ContactFirstName" }, name: "IX_TrainingProviders");
            AddForeignKey("dbo.TrainingProviders", "TrainingProgramId", "dbo.TrainingPrograms", "Id");

            PostDeployment();
        }
        
        public override void Down()
        {
            DropIndex("dbo.GrantApplicationInternalStates", new[] { "Caption" });
            DropIndex("dbo.GrantApplicationExternalStates", new[] { "Caption" });
            DropIndex("dbo.ClaimStates", new[] { "Caption" });
            DropTable("dbo.GrantApplicationInternalStates");
            DropTable("dbo.GrantApplicationExternalStates");
            DropTable("dbo.ClaimStates");

            DropColumn("dbo.GrantApplications", "OrganizationNumberOfEmployeesInBC");
            DropColumn("dbo.ParticipantEnrollments", "ReportedDate");

            AddColumn("dbo.TrainingPrograms", "RequestedTrainingProviderId", c => c.Int());
            AddColumn("dbo.TrainingPrograms", "TrainingProviderId", c => c.Int(nullable: true));

            DropForeignKey("dbo.TrainingProviders", "TrainingProgramId", "dbo.TrainingPrograms");
            DropIndex("dbo.TrainingProviders", "IX_TrainingProviders");
            DropIndex("dbo.TrainingPrograms", "IX_TrainingPrograms");
            DropColumn("dbo.TrainingProviders", "TrainingProgramId");
            CreateIndex("dbo.TrainingProviders", "TrainingProviderTypeId");
            CreateIndex("dbo.TrainingProviders", "GrantApplicationId");
            CreateIndex("dbo.TrainingPrograms", "DeliveryPartnerId");
            CreateIndex("dbo.TrainingPrograms", "TrainingLevelId");
            CreateIndex("dbo.TrainingPrograms", "ExpectedQualificationId");
            CreateIndex("dbo.TrainingPrograms", "SkillFocusId");
            CreateIndex("dbo.TrainingPrograms", "SkillLevelId");
            CreateIndex("dbo.TrainingPrograms", "InDemandOccupationId");
            CreateIndex("dbo.TrainingPrograms", "GrantApplicationId");
            CreateIndex("dbo.TrainingPrograms", "RequestedTrainingProviderId");
            CreateIndex("dbo.TrainingPrograms", "TrainingProviderId");
            AddForeignKey("dbo.TrainingPrograms", "RequestedTrainingProviderId", "dbo.TrainingProviders", "Id");
            AddForeignKey("dbo.TrainingPrograms", "TrainingProviderId", "dbo.TrainingProviders", "Id");
        }
    }
}
