using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.04")]
	public partial class v020404 : ExtendedDbMigration
    {
		public override void Up()
        {
            AddColumn("dbo.Organizations", "BusinessWebsite", c => c.String());
            AddColumn("dbo.Organizations", "BusinessDescription", c => c.String());
            AddColumn("dbo.Organizations", "BusinessTrainingRelevance", c => c.String());
            AddColumn("dbo.TrainingProviders", "AlternativeTrainingOptions", c => c.String());
            AddColumn("dbo.TrainingProviders", "ChoiceOfTrainerOrProgram", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingProviders", "ChoiceOfTrainerOrProgram");
            DropColumn("dbo.TrainingProviders", "AlternativeTrainingOptions");
            DropColumn("dbo.Organizations", "BusinessTrainingRelevance");
            DropColumn("dbo.Organizations", "BusinessDescription");
            DropColumn("dbo.Organizations", "BusinessWebsite");
        }
    }
}
