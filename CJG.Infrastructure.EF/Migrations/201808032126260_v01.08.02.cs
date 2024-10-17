namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.ComponentModel;
    using System.Data.Entity.Migrations;

    [Description("v01.08.02")]
    public partial class v010802 : ExtendedDbMigration
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
