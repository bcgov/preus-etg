namespace CJG.Infrastructure.EF.Migrations
{
	using System.Data.Entity.Migrations;

	internal sealed class Configuration : DbMigrationsConfiguration<CJGContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			CommandTimeout = 10000; 
		}

		protected override void Seed(CJGContext context)
		{
		}
	}
}
