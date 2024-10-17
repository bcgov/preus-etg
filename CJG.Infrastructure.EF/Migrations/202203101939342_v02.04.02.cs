using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.02")]
    public partial class v020402 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachments", "AttachmentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attachments", "AttachmentType");
        }
    }
}
