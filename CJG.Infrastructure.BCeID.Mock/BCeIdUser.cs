using CJG.Core.Entities;
using System;

namespace CJG.Infrastructure.BCeID.Mock
{
    public class BCeIdUser
    {
        public Guid Guid { get; set; }

        public string AccountType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string TelephoneNumber { get; set; }

        public BCeIdBusiness Business { get; set; }

        public static implicit operator User(BCeIdUser user)
        {
            return new User()
            {
                BCeIDGuid = user.Guid,
                AccountType = !String.IsNullOrEmpty(user.AccountType) &&
                                  (BCeIDAccountTypeCodes)Enum.Parse(typeof(BCeIDAccountTypeCodes), user.AccountType, true) ==
                                  BCeIDAccountTypeCodes.Internal
                                    ? Core.Entities.AccountTypes.Internal
                                    : Core.Entities.AccountTypes.External,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.TelephoneNumber,
                Organization = new Organization()
                {
                    BCeIDGuid = user.Business.Guid,
                    LegalName = user.Business.LegalName,
                    DoingBusinessAs = user.Business.DoingBusinessAs,
                    HeadOfficeAddress = new Address()
                    {
                        AddressLine1 = user.Business.Address.AddressLine1,
                        AddressLine2 = user.Business.Address.AddressLine2,
                        City = user.Business.Address.City,
                        RegionId = String.Compare(user.Business.Address.Province, "British Columbia", true) == 0 ? "BC" :
                            String.Compare(user.Business.Address.Province, "Ontario", true) == 0 ? "ON" :
                            String.Compare(user.Business.Address.Province, "Alberta", true) == 0 ? "AB" :
                            String.Compare(user.Business.Address.Province, "Saskatchewan", true) == 0 ? "SK" :
                            String.Compare(user.Business.Address.Province, "Quebec", true) == 0 ? "QC" :
                            String.Compare(user.Business.Address.Province, "Newfoundland", true) == 0 ? "NL" :
                            String.Compare(user.Business.Address.Province, "Manitoba", true) == 0 ? "MB" :
                            String.Compare(user.Business.Address.Province, "New Brunswick", true) == 0 ? "NB" :
                            String.Compare(user.Business.Address.Province, "Prince Edward Island", true) == 0 ? "PE" :
                            String.Compare(user.Business.Address.Province, "Nova Scotia", true) == 0 ? "NS" :
                            String.Compare(user.Business.Address.Province, "Northwest Territories", true) == 0 ? "NT" :
                            String.Compare(user.Business.Address.Province, "Yukon", true) == 0 ? "YT" : "BC",
                        PostalCode = user.Business.Address.PostalCode
                    }
                }
            };
        } 
    }

    public class BCeIdBusiness
    {
        public Guid Guid { get; set; }

        public string LegalName { get; set; }

        public string DoingBusinessAs {get;set;}

        public BCeIdAddress Address { get; set; }
    }

    public class BCeIdAddress
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }
    }
}
