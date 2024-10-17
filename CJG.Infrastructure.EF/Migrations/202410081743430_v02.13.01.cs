using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.13.01")]
	public partial class v021301 : ExtendedDbMigration
    {
		public override void Up()
        {
            AddColumn("dbo.Claims", "ApplicantUpdatedNotes", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "ApplicantUpdatedNotes");
        }
    }
}
