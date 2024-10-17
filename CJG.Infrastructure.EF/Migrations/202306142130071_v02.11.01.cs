using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.11.01")]
    public partial class v021101 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EligibleCosts", "ExpenseExplanation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EligibleCosts", "ExpenseExplanation");
        }
    }
}
