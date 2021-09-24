namespace CJG.Infrastructure.EF.Migrations
{  
    using System.ComponentModel;

    [Description("v01.01.00")]
    public partial class v110 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();
            AddColumn("dbo.TrainingPrograms", "TotalEstimatedReimbursement", c => c.Single(nullable: false));
            AddColumn("dbo.TrainingPrograms", "AgreedCommitment", c => c.Single(nullable: false));
            PostDeployment();
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingPrograms", "AgreedCommitment");
            DropColumn("dbo.TrainingPrograms", "TotalEstimatedReimbursement");
        }
    }
}
