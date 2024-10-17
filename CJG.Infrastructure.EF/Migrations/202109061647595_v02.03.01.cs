namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v02.03.01")]
	public partial class v020301 : ExtendedDbMigration
    {
        public override void Up()
        {
			AddColumn("dbo.ParticipantForms", "Approved", c => c.Boolean());
			AddColumn("dbo.ParticipantForms", "Attended", c => c.Boolean());
			AddColumn("dbo.GrantApplications", "RequireAllParticipantsBeforeSubmission", c => c.Boolean(nullable: false));

			PostDeployment();
		}
        
        public override void Down()
        {
			DropColumn("dbo.ParticipantForms", "Approved");
			DropColumn("dbo.ParticipantForms", "Attended");
			DropColumn("dbo.GrantApplications", "RequireAllParticipantsBeforeSubmission");
		}
    }
}
