using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.17.05")]
    public partial class v021705 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "PreviousEmployerFullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "PreviousEmployerFullName");
        }
    }
}
