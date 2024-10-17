using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.09.00")]
    public partial class v020900 : ExtendedDbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.NationalOccupationalClassifications", "IX_NaIndustryClassificationSystem");
            DropIndex("dbo.NationalOccupationalClassifications", "IX_NationalOccupationalClassification");
            AddColumn("dbo.NationalOccupationalClassifications", "NOCVersion", c => c.String(nullable: false, maxLength: 10));
            CreateIndex("dbo.NationalOccupationalClassifications", new[] { "Code", "Level", "NOCVersion" }, unique: true, name: "IX_NaIndustryClassificationSystem");

			PostDeployment();
        }
        
        public override void Down()
        {
            DropIndex("dbo.NationalOccupationalClassifications", "IX_NaIndustryClassificationSystem");
            DropColumn("dbo.NationalOccupationalClassifications", "NOCVersion");
            CreateIndex("dbo.NationalOccupationalClassifications", new[] { "Left", "Right", "Level" }, unique: true, name: "IX_NationalOccupationalClassification");
            CreateIndex("dbo.NationalOccupationalClassifications", "Code", unique: true, name: "IX_NaIndustryClassificationSystem");
        }
    }
}
