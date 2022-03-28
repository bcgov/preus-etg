using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.05")]
	public partial class v020405 : ExtendedDbMigration
	{
        public override void Up()
        {
            AddColumn("dbo.Organizations", "BusinessWebsite", c => c.String());
            AddColumn("dbo.Organizations", "BusinessDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "BusinessDescription");
            DropColumn("dbo.Organizations", "BusinessWebsite");
        }
    }
}
