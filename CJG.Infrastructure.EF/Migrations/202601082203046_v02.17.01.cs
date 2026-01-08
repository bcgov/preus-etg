using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.17.01")]
	public partial class v021701 : ExtendedDbMigration
	{
		public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "MultipleEmploymentPositions", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "MultipleEmploymentPositions");
        }
    }
}
