namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.05.01")]
    public partial class v010501 : ExtendedDbMigration
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
