using System.ComponentModel;

namespace CJG.Infrastructure.EF.Migrations
{
	[Description("v02.04.04")]
    public partial class v020404 : ExtendedDbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrganizationBusinessLicenseDocuments",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false),
                        BusinessLicenseDocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.BusinessLicenseDocumentId })
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Attachments", t => t.BusinessLicenseDocumentId)
                .Index(t => t.OrganizationId)
                .Index(t => t.BusinessLicenseDocumentId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganizationBusinessLicenseDocuments", "BusinessLicenseDocumentId", "dbo.Attachments");
            DropForeignKey("dbo.OrganizationBusinessLicenseDocuments", "OrganizationId", "dbo.Organizations");
            DropIndex("dbo.OrganizationBusinessLicenseDocuments", new[] { "BusinessLicenseDocumentId" });
            DropIndex("dbo.OrganizationBusinessLicenseDocuments", new[] { "OrganizationId" });
            DropTable("dbo.OrganizationBusinessLicenseDocuments");
        }
    }
}
