using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.10.02")]
    public partial class v021002 : ExtendedDbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PrioritizationPostalCodes", "PostalCode", c => c.String(maxLength: 10));

            PostDeployment();
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PrioritizationPostalCodes", "PostalCode", c => c.String());
        }
    }
}
