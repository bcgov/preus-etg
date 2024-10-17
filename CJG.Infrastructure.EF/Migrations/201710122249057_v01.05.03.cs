using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
    [Description("v01.05.03")]
    public partial class v010503 : ExtendedDbMigration
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
