namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;
    
    [Description("v01.07.02")]
    public partial class v010702 : ExtendedDbMigration
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
