using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.06.03")]
	public partial class v020603 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingProviderInventory", "RiskFlag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingProviderInventory", "RiskFlag");
        }
    }
}
