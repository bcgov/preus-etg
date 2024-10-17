namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;
    
    [Description("v01.08.01")]
    public partial class v010801 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropForeignKey("dbo.GrantApplications", "ApplicantDeclarationId", "dbo.ApplicantDeclarations");
            DropForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents");
            DropForeignKey("dbo.GrantStreams", "GrantProgramId", "dbo.GrantPrograms");
            DropIndex("dbo.GrantPrograms", "IX_GrantProgram");
            DropIndex("dbo.GrantApplications", new[] { "ApplicantDeclarationId" });
            DropIndex("dbo.ApplicantDeclarations", "IX_ApplicantDeclaration");
            DropIndex("dbo.ParticipantForms", new[] { "ParticipantConsentId" });
            DropIndex("dbo.ParticipantConsents", "IX_ParticipantConsent");
            DropIndex("dbo.GrantStreams", "IX_GrantStream");
            AddColumn("dbo.GrantPrograms", "ProgramCode", c => c.String(nullable: false, maxLength: 5));
            AddColumn("dbo.GrantPrograms", "BatchRequestDescription", c => c.String(maxLength: 2000));
            AddColumn("dbo.GrantPrograms", "EligibilityDescription", c => c.String(maxLength: 2000));
            AddColumn("dbo.PaymentRequests", "GLClientNumber", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.PaymentRequests", "GLRESP", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.PaymentRequests", "GLServiceLine", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.PaymentRequests", "GLSTOB", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.PaymentRequests", "GLProjectCode", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.PaymentRequestBatches", "RequestedBy", c => c.String(maxLength: 250));
            AddColumn("dbo.PaymentRequestBatches", "ProgramPhone", c => c.String(maxLength: 50));
            AddColumn("dbo.PaymentRequestBatches", "DocumentPrefix", c => c.String(maxLength: 5));
            AddColumn("dbo.PaymentRequestBatches", "ExpenseAuthorityId", c => c.Int());
            AddColumn("dbo.PaymentRequestBatches", "GrantProgramId", c => c.Int());
            AlterColumn("dbo.GrantStreams", "GrantProgramId", c => c.Int(nullable: false));
            AlterColumn("dbo.GrantStreams", "Name", c => c.String(nullable: false, maxLength: 250));
            CreateIndex("dbo.GrantPrograms", new[] { "AccountCodeId", "Name", "ProgramCode", "State" }, name: "IX_GrantProgram");
            CreateIndex("dbo.GrantPrograms", "Name", unique: true, name: "IX_GrantProgram_Name");
            CreateIndex("dbo.PaymentRequestBatches", "ExpenseAuthorityId");
            CreateIndex("dbo.PaymentRequestBatches", "GrantProgramId");
            CreateIndex("dbo.GrantStreams", new[] { "GrantProgramId", "AccountCodeId", "IsActive" }, name: "IX_GrantStream");
            AddForeignKey("dbo.PaymentRequestBatches", "ExpenseAuthorityId", "dbo.InternalUsers", "Id");
            AddForeignKey("dbo.PaymentRequestBatches", "GrantProgramId", "dbo.GrantPrograms", "Id");
            AddForeignKey("dbo.GrantStreams", "GrantProgramId", "dbo.GrantPrograms", "Id", cascadeDelete: true);
            DropColumn("dbo.GrantPrograms", "Abbreviation");
            DropColumn("dbo.GrantPrograms", "Description");
            DropColumn("dbo.GrantApplications", "ApplicantDeclarationId");
            DropColumn("dbo.ParticipantForms", "ParticipantConsentId");
            DropTable("dbo.ApplicantDeclarations");
            DropTable("dbo.ParticipantConsents");

            PostDeployment();

            CreateIndex("dbo.GrantPrograms", "ProgramCode", unique: true, name: "IX_GrantProgram_Code");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ParticipantConsents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicantDeclarations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Body = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()", storeType: "datetime2"),
                        DateUpdated = c.DateTime(storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ParticipantForms", "ParticipantConsentId", c => c.Int());
            AddColumn("dbo.GrantApplications", "ApplicantDeclarationId", c => c.Int());
            AddColumn("dbo.GrantPrograms", "Description", c => c.String(maxLength: 2000));
            AddColumn("dbo.GrantPrograms", "Abbreviation", c => c.String(nullable: false, maxLength: 5));
            DropForeignKey("dbo.GrantStreams", "GrantProgramId", "dbo.GrantPrograms");
            DropForeignKey("dbo.PaymentRequestBatches", "GrantProgramId", "dbo.GrantPrograms");
            DropForeignKey("dbo.PaymentRequestBatches", "ExpenseAuthorityId", "dbo.InternalUsers");
            DropIndex("dbo.GrantStreams", "IX_GrantStream");
            DropIndex("dbo.PaymentRequestBatches", new[] { "GrantProgramId" });
            DropIndex("dbo.PaymentRequestBatches", new[] { "ExpenseAuthorityId" });
            DropIndex("dbo.GrantPrograms", "IX_GrantProgram_Code");
            DropIndex("dbo.GrantPrograms", "IX_GrantProgram_Name");
            DropIndex("dbo.GrantPrograms", "IX_GrantProgram");
            AlterColumn("dbo.GrantStreams", "Name", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.GrantStreams", "GrantProgramId", c => c.Int());
            DropColumn("dbo.PaymentRequestBatches", "GrantProgramId");
            DropColumn("dbo.PaymentRequestBatches", "ExpenseAuthorityId");
            DropColumn("dbo.PaymentRequestBatches", "DocumentPrefix");
            DropColumn("dbo.PaymentRequestBatches", "ProgramPhone");
            DropColumn("dbo.PaymentRequestBatches", "RequestedBy");
            DropColumn("dbo.PaymentRequests", "GLProjectCode");
            DropColumn("dbo.PaymentRequests", "GLSTOB");
            DropColumn("dbo.PaymentRequests", "GLServiceLine");
            DropColumn("dbo.PaymentRequests", "GLRESP");
            DropColumn("dbo.PaymentRequests", "GLClientNumber");
            DropColumn("dbo.GrantPrograms", "EligibilityDescription");
            DropColumn("dbo.GrantPrograms", "BatchRequestDescription");
            DropColumn("dbo.GrantPrograms", "ProgramCode");
            CreateIndex("dbo.GrantStreams", new[] { "GrantProgramId", "AccountCodeId", "IsActive" }, name: "IX_GrantStream");
            CreateIndex("dbo.ParticipantConsents", "IsActive", name: "IX_ParticipantConsent");
            CreateIndex("dbo.ParticipantForms", "ParticipantConsentId");
            CreateIndex("dbo.ApplicantDeclarations", "IsActive", name: "IX_ApplicantDeclaration");
            CreateIndex("dbo.GrantApplications", "ApplicantDeclarationId");
            CreateIndex("dbo.GrantPrograms", new[] { "AccountCodeId", "Name", "Abbreviation", "State" }, unique: true, name: "IX_GrantProgram");
            AddForeignKey("dbo.GrantStreams", "GrantProgramId", "dbo.GrantPrograms", "Id");
            AddForeignKey("dbo.ParticipantForms", "ParticipantConsentId", "dbo.ParticipantConsents", "Id");
            AddForeignKey("dbo.GrantApplications", "ApplicantDeclarationId", "dbo.ApplicantDeclarations", "Id");
        }
    }
}
