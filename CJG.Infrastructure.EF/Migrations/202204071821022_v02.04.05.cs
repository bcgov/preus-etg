using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.05")]
	public partial class v020405 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingPrograms", "BusinessTrainingRelevance", c => c.String());
            DropColumn("dbo.Organizations", "BusinessTrainingRelevance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Organizations", "BusinessTrainingRelevance", c => c.String());
            DropColumn("dbo.TrainingPrograms", "BusinessTrainingRelevance");
        }
    }
}
