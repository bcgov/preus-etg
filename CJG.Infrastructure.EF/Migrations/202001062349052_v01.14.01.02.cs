namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
	using System.ComponentModel;

	[Description("v01.14.01.02")]
	public partial class v01140102 : ExtendedDbMigration
	{
        public override void Up()
        {
            //CreateTable(
            //    "dbo.ClaimParticipants1",
            //    c => new
            //        {
            //            ClaimId = c.Int(nullable: false),
            //            ClaimVersion = c.Int(nullable: false),
            //            ParticipantFormId = c.Int(nullable: false),
            //            DateAdded = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
            //            DateUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //            Claim_Id = c.Int(),
            //            Claim_ClaimVersion = c.Int(),
            //        })
            //    .PrimaryKey(t => new { t.ClaimId, t.ClaimVersion, t.ParticipantFormId })
            //    .ForeignKey("dbo.Claims", t => new { t.Claim_Id, t.Claim_ClaimVersion })
            //    .Index(t => new { t.Claim_Id, t.Claim_ClaimVersion });
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.ClaimParticipants1", new[] { "Claim_Id", "Claim_ClaimVersion" }, "dbo.Claims");
            //DropIndex("dbo.ClaimParticipants1", new[] { "Claim_Id", "Claim_ClaimVersion" });
            //DropTable("dbo.ClaimParticipants1");
        }
    }
}
