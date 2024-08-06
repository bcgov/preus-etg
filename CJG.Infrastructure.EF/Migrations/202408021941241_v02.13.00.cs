using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.13.00")]
    public partial class v021300 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "ApplicantNotes", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "ApplicantNotes");
        }
    }
}
