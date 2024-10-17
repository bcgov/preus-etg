using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.11.02")]
    public partial class v021102 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParticipantInvitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrantApplicationId = c.Int(),
                        ParticipantFormId = c.Int(),
                        ParticipantInvitationStatus = c.Int(nullable: false),
                        IndividualKey = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EmailAddress = c.String(),
                        ExpectedParticipantOutcome = c.Int(nullable: false),
                        DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GrantApplications", t => t.GrantApplicationId)
                .ForeignKey("dbo.ParticipantForms", t => t.ParticipantFormId)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.ParticipantFormId);
            
            AddColumn("dbo.ParticipantForms", "IndividualKey", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParticipantInvitations", "ParticipantFormId", "dbo.ParticipantForms");
            DropForeignKey("dbo.ParticipantInvitations", "GrantApplicationId", "dbo.GrantApplications");
            DropIndex("dbo.ParticipantInvitations", new[] { "ParticipantFormId" });
            DropIndex("dbo.ParticipantInvitations", new[] { "GrantApplicationId" });
            DropColumn("dbo.ParticipantForms", "IndividualKey");
            DropTable("dbo.ParticipantInvitations");
        }
    }
}
