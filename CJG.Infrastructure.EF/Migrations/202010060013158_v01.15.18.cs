namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v011518 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParticipantForms", "ReceivingEIBenefit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParticipantForms", "ReceivingEIBenefit");
        }
    }
}
