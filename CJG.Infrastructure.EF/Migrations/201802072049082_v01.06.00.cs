namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;
    
    [Description("v01.06.00")]
    public partial class v010600 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropColumn("dbo.GrantOpeningFinancials", "ClaimsReceived");
            DropColumn("dbo.GrantOpeningFinancials", "ClaimsRecievedCount");
            DropColumn("dbo.GrantOpeningFinancials", "PaymentRequests");
            DropColumn("dbo.GrantOpeningIntakes", "ReductionsCount");
            DropColumn("dbo.GrantOpeningIntakes", "CommitmentCount");
            DropColumn("dbo.GrantOpeningIntakes", "CommitmentAmt");

            PostDeployment();
        }
        
        public override void Down()
        {
            AddColumn("dbo.GrantOpeningIntakes", "CommitmentAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.GrantOpeningIntakes", "CommitmentCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningIntakes", "ReductionsCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "PaymentRequests", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.GrantOpeningFinancials", "ClaimsRecievedCount", c => c.Int(nullable: false));
            AddColumn("dbo.GrantOpeningFinancials", "ClaimsReceived", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
