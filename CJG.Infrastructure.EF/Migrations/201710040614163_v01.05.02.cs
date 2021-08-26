namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.05.02")]
    public partial class v010502 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();
            DropColumn("dbo.GrantApplications", "ApplicantEmail");
            AddColumn("dbo.ClaimEligibleCosts", "SourceId", c => c.Int());
            CreateIndex("dbo.ClaimEligibleCosts", "SourceId");
            AddForeignKey("dbo.ClaimEligibleCosts", "SourceId", "dbo.ClaimEligibleCosts", "Id");
            PostDeployment();
        }
        
        public override void Down()
        {
            AddColumn("dbo.GrantApplications", "ApplicantEmail", c => c.String(nullable: false, maxLength: 500));
            DropForeignKey("dbo.ClaimEligibleCosts", "SourceId", "dbo.ClaimEligibleCosts");
            DropIndex("dbo.ClaimEligibleCosts", new[] { "SourceId" });
            DropColumn("dbo.ClaimEligibleCosts", "SourceId");
        }
    }
}