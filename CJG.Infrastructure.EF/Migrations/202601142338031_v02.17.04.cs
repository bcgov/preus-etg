using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.17.04")]
    public partial class v021704 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "PreviousAvgHoursPerWeek", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "PreviousAvgHoursPerWeek");
        }
    }
}
