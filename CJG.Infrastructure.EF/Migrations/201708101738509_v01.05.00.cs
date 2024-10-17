namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.05.00")]
    public partial class v010500 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropIndex("dbo.Claims", "IX_Claim");

            AddColumn("dbo.GrantOpeningFinancials", "OutstandingCommitmentCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "ClaimsAssessedCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "CurrentClaimCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "AssessedCommitmentsCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "CancellationsCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "ClaimsRecievedCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "ClaimsDeniedCount", c => c.Int(nullable: false));
            AddColumn("dbo.ParticipantEnrollments", "ClaimReported", c => c.Boolean(nullable: false));

            RenameColumn("dbo.Claims", "FileNumber", "ClaimNumber");
            RenameColumn("dbo.Claims", "AssessorNotes", "ClaimAssessmentNotes");
            RenameColumn("dbo.ClaimEligibleCosts", "ClaimMaxReimbursementCost", "ClaimMaxParticipantReimbursementCost");
            RenameColumn("dbo.ClaimEligibleCosts", "ClaimEmployerContribution", "ClaimParticipantEmployerContribution");
            RenameColumn("dbo.ClaimEligibleCosts", "AssessedMaxReimbursementCost", "AssessedMaxParticipantReimbursementCost");
            RenameColumn("dbo.ClaimEligibleCosts", "AssessedEmployerContribution", "AssessedParticipantEmployerContribution");

            AddColumn("dbo.Claims", "EligibilityAssessmentNotes", c => c.String(maxLength: 2000));
            AddColumn("dbo.Claims", "ReimbursementAssessmentNotes", c => c.String(maxLength: 2000));
            AlterColumn("dbo.Claims", "ClaimAssessmentNotes", c => c.String(maxLength: 2000));

            CreateIndex("dbo.Claims", new[] { "AssessorId", "ClaimState", "ClaimNumber", "DateSubmitted", "DateAssessed" }, name: "IX_Claim");

            PostDeployment();
        }
        
        public override void Down()
        {
            DropIndex("dbo.Claims", "IX_Claim");
            DropColumn("dbo.GrantOpeningFinancials", "CurrentClaimCount");
            DropColumn("dbo.GrantOpeningFinancials", "ClaimsAssessedCount");
            DropColumn("dbo.GrantOpeningFinancials", "OutstandingCommitmentCount");
            DropColumn("dbo.GrantOpeningFinancials", "ClaimsDeniedCount");
            DropColumn("dbo.GrantOpeningFinancials", "ClaimsRecievedCount");
            DropColumn("dbo.GrantOpeningFinancials", "CancellationsCount");
            DropColumn("dbo.GrantOpeningFinancials", "AssessedCommitmentsCount");
            DropColumn("dbo.ParticipantEnrollments", "ClaimReported");
            AlterColumn("dbo.Claims", "ClaimAssessmentNotes", c => c.String());
            DropColumn("dbo.Claims", "ReimbursementAssessmentNotes");
            DropColumn("dbo.Claims", "EligibilityAssessmentNotes");
            RenameColumn("dbo.Claims", "ClaimNumber", "FileNumber");
            RenameColumn("dbo.Claims", "ClaimAssessmentNotes", "AssessorNotes");
            RenameColumn("dbo.ClaimEligibleCosts", "ClaimMaxParticipantReimbursementCost", "ClaimMaxReimbursementCost");
            RenameColumn("dbo.ClaimEligibleCosts", "ClaimParticipantEmployerContribution", "ClaimEmployerContribution");
            RenameColumn("dbo.ClaimEligibleCosts", "AssessedMaxParticipantReimbursementCost", "AssessedMaxReimbursementCost");
            RenameColumn("dbo.ClaimEligibleCosts", "AssessedParticipantEmployerContribution", "AssessedEmployerContribution");
            CreateIndex("dbo.Claims", new[] { "AssessorId", "ClaimState", "FileNumber", "DateSubmitted", "DateAssessed" }, name: "IX_Claim");
        }
    }
}
