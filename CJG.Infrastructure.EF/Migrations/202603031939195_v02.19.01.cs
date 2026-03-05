using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
    [Description("v02.19.01")]
    public partial class v021901 : ExtendedDbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingPrograms", "TrainingObjectiveId", c => c.Int());
            CreateIndex("dbo.TrainingPrograms", "TrainingObjectiveId");
            AddForeignKey("dbo.TrainingPrograms", "TrainingObjectiveId", "dbo.TrainingObjectives", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingPrograms", "TrainingObjectiveId", "dbo.TrainingObjectives");
            DropIndex("dbo.TrainingPrograms", new[] { "TrainingObjectiveId" });
            DropColumn("dbo.TrainingPrograms", "TrainingObjectiveId");
        }
    }
}
