namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v011502 : DbMigration
    {
        public override void Up()
        {
			AddColumn("dbo.GrantApplications", "InsuranceConfirmed", c => c.Boolean(nullable: true, defaultValueSql: "0"));
		}
        
        public override void Down()
        {
			DropColumn("dbo.GrantApplications", "InsuranceConfirmed");
		}
    }
}
