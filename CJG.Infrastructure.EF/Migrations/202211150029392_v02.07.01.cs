using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.07.01")]
    public partial class v020701 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingProviders", "OutOfProvinceLocationRationale", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingProviders", "OutOfProvinceLocationRationale");
        }
    }
}
