namespace CJG.Infrastructure.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v020404 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingProviders", "AlternativeTrainingOptions", c => c.String());
            AddColumn("dbo.TrainingProviders", "ChoiceOfTrainerOrProgram", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingProviders", "ChoiceOfTrainerOrProgram");
            DropColumn("dbo.TrainingProviders", "AlternativeTrainingOptions");
        }
    }
}
