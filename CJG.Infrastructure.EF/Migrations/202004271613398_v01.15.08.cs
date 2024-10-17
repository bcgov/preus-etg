namespace CJG.Infrastructure.EF.Migrations
{
    using System;
	using System.ComponentModel;
	using System.Data.Entity.Migrations;

	[Description("v01.15.08")]
	public partial class v011508 : ExtendedDbMigration
	{
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationParticipantEmploymentStatuses",
                c => new
                    {
                        GrantApplicationId = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GrantApplicationId, t.Id })
                .ForeignKey("dbo.ProgramDescriptions", t => t.GrantApplicationId)
                .ForeignKey("dbo.ParticipantEmploymentStatus", t => t.Id)
                .Index(t => t.GrantApplicationId)
                .Index(t => t.Id);

			PostDeployment();
		}

        public override void Down()
        {
            DropForeignKey("dbo.ApplicationParticipantEmploymentStatuses", "Id", "dbo.ParticipantEmploymentStatus");
            DropForeignKey("dbo.ApplicationParticipantEmploymentStatuses", "GrantApplicationId", "dbo.ProgramDescriptions");
            DropIndex("dbo.ApplicationParticipantEmploymentStatuses", new[] { "Id" });
            DropIndex("dbo.ApplicationParticipantEmploymentStatuses", new[] { "GrantApplicationId" });
            DropTable("dbo.ApplicationParticipantEmploymentStatuses");
        }
    }
}
