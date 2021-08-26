namespace CJG.Infrastructure.EF.Migrations
{
    using System.ComponentModel;

    [Description("v01.04.00")]
    public partial class v010400 : ExtendedDbMigration
    {
        public override void Up()
        {
            PreDeployment();

            DropIndex("dbo.Streams", "IX_Stream");

            PostDeployment();
        }

        public override void Down()
        {
            CreateIndex("dbo.Streams", "Name", unique: true, name: "IX_Stream");
        }
    }
}