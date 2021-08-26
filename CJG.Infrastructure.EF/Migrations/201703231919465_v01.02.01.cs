namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.02.01")]
    public partial class v121 : ExtendedDbMigration
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
