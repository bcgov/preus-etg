using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CJG.Infrastructure.BCeID.Mock
{
	public class MockBCeIDRepository : IBCeIDService
    {
        public User GetInternalUserInfo(Guid userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
        {
            return GetUserInfo(userId);
        }

        public User GetBusinessUserInfo(Guid userId, Guid requesterUserId, BCeIDAccountTypeCodes requesterAccountType)
        {
            //
            // Launched at external user login
            //
            return GetUserInfo(userId);
        }

        public User GetBusinessUserInfo(string userId, Guid requesterUserId, CJG.Core.Entities.BCeIDAccountTypeCodes requesterAccountType)
        {
            return GetUserInfo(Guid.NewGuid());
        }

        private User GetUserInfo(Guid userId)
        {
            var filePath = Path.Combine(HttpRuntime.AppDomainAppPath, String.Format(@"bin\MockBCeIDData\{0}.xml", userId.ToString()));
            if (File.Exists(filePath))
            {
                return DeserializeUser(filePath);
            }
            else
            {
                // Hack for Tim's testing on ISB infrastructure until integration with BCeID web service is working
                Address businessAddress = new Address
                {
                    Id = 2,
                    AddressLine1 = "333 Seymour St.",
                    AddressLine2 = "Suite 1000",
                    City = "Vancouver",
                    RegionId = "BC",
                    PostalCode = "V5B 5A6"
                };

                Organization organization = new Organization
                {
                    BCeIDGuid = new Guid(),
                    LegalName = "FCV Interactive (Vancouver)",
                    DoingBusinessAs = "FCV Vancouver",
                    HeadOfficeAddress = businessAddress,
                    OrganizationTypeId = (int)OrganizationTypeCodes.Default,
                    YearEstablished = DateTime.UtcNow.Year
                };

                User dummyUser = new User
                {
                    BCeID = "TestUser",
                    BCeIDGuid = userId,
                    AccountType = AccountTypes.External,
                    Salutation = "Mr.",
                    FirstName = "New",
                    LastName = "User",
                    EmailAddress = "new.user@fcvinteractive.com",
                    PhoneNumber = "(604) 123-4567",
                    Organization = organization,
                    IsOrganizationProfileAdministrator = true
                };

                return dummyUser;
            }
        }

        private static User DeserializeUser(string filePath)
        {
            // A FileStream is needed to read the XML document.
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var root = new XmlRootAttribute() { ElementName = "User", IsNullable = false };
                XmlSerializer serializer = new
                XmlSerializer(typeof(BCeIdUser), root);

                XmlReader reader = XmlReader.Create(fs);
                var user = (BCeIdUser)serializer.Deserialize(reader);
                fs.Close();
                return (User)user;
            }
        }
        public User GetInternalUserInfo(string idir, Guid requesterUserGuid)
        {
            return new User();
        }
    }
}