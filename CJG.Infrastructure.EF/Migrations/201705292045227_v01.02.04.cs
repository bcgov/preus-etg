namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.02.04")]
    public partial class v010204 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            AddColumn("dbo.ClaimEligibleCosts", "AddedByAssessor", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.EligibleCosts", "AddedByAssessor", c => c.Boolean(nullable: false, defaultValue: false));

            DropConstraint("TrainingPrograms", "TotalEstimatedReimbursement");
            DropConstraint("TrainingPrograms", "AgreedCommitment");

            AlterColumn("dbo.GrantApplications", "OrganizationAnnualTrainingBudget", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.GrantApplications", "MaxReimbursementAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Claims", "TotalClaimReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Claims", "TotalAssessedReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimMaxParticipantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimMaxReimbursementCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimEmployerContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedMaxParticipantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedMaxReimbursementCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedEmployerContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "EstimatedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "EstimatedParticipantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "EstimatedReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "EstimatedEmployerContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "AgreedMaxCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "AgreedMaxParticipantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "AgreedMaxReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.EligibleCosts", "AgreedEmployerContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TrainingPrograms", "TotalEstimatedCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TrainingPrograms", "TotalEstimatedReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TrainingPrograms", "TotalAgreedMaxCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TrainingPrograms", "AgreedCommitment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Organizations", "AnnualTrainingBudget", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantCosts", "ClaimParticipantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantCosts", "ClaimReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantCosts", "ClaimEmployerContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantCosts", "AssessedParticipantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantCosts", "AssessedReimbursement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantCosts", "AssessedEmployerContribution", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ParticipantForms", "HourlyWage", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Payments", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpenings", "IntakeTargetAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpenings", "BudgetAllocationAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "CurrentReservations", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "AssessedCommitments", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "OutstandingCommitments", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "Cancellations", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "ClaimsReceived", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "ClaimsAssessed", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "CurrentClaims", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "ClaimsDenied", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningFinancials", "PaymentRequests", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "NewAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "PendingAssessmentAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "UnderAssessmentAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "DeniedAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "WithdrawnAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "ReductionsAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.GrantOpeningIntakes", "CommitmentAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Streams", "MaxReimbursementAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));

            DropPrimaryKey("dbo.RateFormats");
            AlterColumn("dbo.GrantApplications", "ReimbursementRate", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanDeniedRate", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanWithdrawnRate", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanReductionRate", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanSlippageRate", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanCancellationRate", c => c.Double(nullable: false));
            AlterColumn("dbo.Streams", "ReimbursementRate", c => c.Double(nullable: false));
            AlterColumn("dbo.Streams", "DefaultDeniedRate", c => c.Double(nullable: false));
            AlterColumn("dbo.Streams", "DefaultWithdrawnRate", c => c.Double(nullable: false));
            AlterColumn("dbo.Streams", "DefaultReductionRate", c => c.Double(nullable: false));
            AlterColumn("dbo.Streams", "DefaultSlippageRate", c => c.Double(nullable: false));
            AlterColumn("dbo.Streams", "DefaultCancellationRate", c => c.Double(nullable: false));
            AlterColumn("dbo.StreamFiscals", "ClaimSlippageRate", c => c.Double(nullable: false));
            AlterColumn("dbo.StreamFiscals", "AgreementCancellationRate", c => c.Double(nullable: false));
            AlterColumn("dbo.StreamFiscals", "AgreementSlippageRate", c => c.Double(nullable: false));
            AlterColumn("dbo.RateFormats", "Rate", c => c.Double(nullable: false));
            AddPrimaryKey("dbo.RateFormats", "Rate");

            PostDeployment();
        }

        public override void Down()
        {
            DropColumn("dbo.EligibleCosts", "AddedByAssessor");
            DropColumn("dbo.ClaimEligibleCosts", "AddedByAssessor");

            AlterColumn("dbo.Streams", "MaxReimbursementAmt", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "CommitmentAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "ReductionsAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "WithdrawnAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "DeniedAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "UnderAssessmentAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "PendingAssessmentAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningIntakes", "NewAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "PaymentRequests", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "ClaimsDenied", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "CurrentClaims", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "ClaimsAssessed", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "ClaimsReceived", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "Cancellations", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "OutstandingCommitments", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "AssessedCommitments", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpeningFinancials", "CurrentReservations", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "BudgetAllocationAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.GrantOpenings", "IntakeTargetAmt", c => c.Double(nullable: false));
            AlterColumn("dbo.Payments", "Amount", c => c.Single(nullable: false));
            AlterColumn("dbo.ParticipantForms", "HourlyWage", c => c.Single());
            AlterColumn("dbo.ParticipantCosts", "AssessedEmployerContribution", c => c.Single(nullable: false));
            AlterColumn("dbo.ParticipantCosts", "AssessedReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.ParticipantCosts", "AssessedParticipantCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ParticipantCosts", "ClaimEmployerContribution", c => c.Single(nullable: false));
            AlterColumn("dbo.ParticipantCosts", "ClaimReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.ParticipantCosts", "ClaimParticipantCost", c => c.Single(nullable: false));
            AlterColumn("dbo.Organizations", "AnnualTrainingBudget", c => c.Single());
            AlterColumn("dbo.TrainingPrograms", "AgreedCommitment", c => c.Single(nullable: false));
            AlterColumn("dbo.TrainingPrograms", "TotalAgreedMaxCost", c => c.Single(nullable: false));
            AlterColumn("dbo.TrainingPrograms", "TotalEstimatedReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.TrainingPrograms", "TotalEstimatedCost", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "AgreedEmployerContribution", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "AgreedMaxReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "AgreedMaxParticipantCost", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "AgreedMaxCost", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "EstimatedEmployerContribution", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "EstimatedReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "EstimatedParticipantCost", c => c.Single(nullable: false));
            AlterColumn("dbo.EligibleCosts", "EstimatedCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedEmployerContribution", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedMaxReimbursementCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedMaxParticipantCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "AssessedCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimEmployerContribution", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimMaxReimbursementCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimMaxParticipantCost", c => c.Single(nullable: false));
            AlterColumn("dbo.ClaimEligibleCosts", "ClaimCost", c => c.Single(nullable: false));
            AlterColumn("dbo.Claims", "TotalAssessedReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.Claims", "TotalClaimReimbursement", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantApplications", "MaxReimbursementAmt", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantApplications", "OrganizationAnnualTrainingBudget", c => c.Single());

            DropPrimaryKey("dbo.RateFormats");
            AlterColumn("dbo.RateFormats", "Rate", c => c.Single(nullable: false));
            AlterColumn("dbo.StreamFiscals", "AgreementSlippageRate", c => c.Single(nullable: false));
            AlterColumn("dbo.StreamFiscals", "AgreementCancellationRate", c => c.Single(nullable: false));
            AlterColumn("dbo.StreamFiscals", "ClaimSlippageRate", c => c.Single(nullable: false));
            AlterColumn("dbo.Streams", "DefaultCancellationRate", c => c.Single(nullable: false));
            AlterColumn("dbo.Streams", "DefaultSlippageRate", c => c.Single(nullable: false));
            AlterColumn("dbo.Streams", "DefaultReductionRate", c => c.Single(nullable: false));
            AlterColumn("dbo.Streams", "DefaultWithdrawnRate", c => c.Single(nullable: false));
            AlterColumn("dbo.Streams", "DefaultDeniedRate", c => c.Single(nullable: false));
            AlterColumn("dbo.Streams", "ReimbursementRate", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanCancellationRate", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanSlippageRate", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanReductionRate", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanWithdrawnRate", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantOpenings", "PlanDeniedRate", c => c.Single(nullable: false));
            AlterColumn("dbo.GrantApplications", "ReimbursementRate", c => c.Single(nullable: false));
            AddPrimaryKey("dbo.RateFormats", "Rate");
        }
    }
}
