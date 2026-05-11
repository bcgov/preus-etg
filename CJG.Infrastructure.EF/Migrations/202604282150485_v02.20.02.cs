using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.20.02")]
	public partial class v022002 : ExtendedDbMigration
    {
		public override void Up()
        {
            AddColumn("dbo.TrainingProviders", "IsPublicPostSecondarySchool", c => c.Boolean());
            AddColumn("dbo.TrainingProviders", "PublicPostSecondarySchoolId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingProviders", "PublicPostSecondarySchoolId");
            DropColumn("dbo.TrainingProviders", "IsPublicPostSecondarySchool");
        }
    }
}
