using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.01")]
    public partial class v020401 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "JobTitleBefore", c => c.String(maxLength: 2000));
            AddColumn("dbo.ParticipantForms", "JobTitleFuture", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "JobTitleFuture");
            DropColumn("dbo.ParticipantForms", "JobTitleBefore");
        }
    }
}
