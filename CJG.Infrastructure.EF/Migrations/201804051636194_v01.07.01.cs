namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.ComponentModel;
    using System.Data.Entity.Migrations;

    [Description("v01.07.01")]
    public partial class v010701 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            PostDeployment();
        }
        
        public override void Down()
        {
        }
    }
}
