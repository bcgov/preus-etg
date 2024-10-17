using CJG.Core.Entities;
using System;
using CJG.Core.Entities.Attributes;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class ParticipantFormViewModel
    {
        #region Properties
        public int Id { get; set; }

        [NameValidation]
        public string FirstName { get; set; }

        [NameValidation]
        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneExtension1 { get; set; }

        public string PrimaryCity { get; set; }

        public bool ClaimReported { get; set; }

        public bool IsIncludedInClaim { get; set; }

        public DateTime DateAdded { get; set; }

        public byte[] RowVersion { get; set; }

		public bool? Attended { get; set; }
		#endregion

		#region Constructors
		public ParticipantFormViewModel()
        {

        }

        public ParticipantFormViewModel(ParticipantForm participantForm)
        {
            if (participantForm == null)
                throw new ArgumentNullException(nameof(participantForm));

            this.Id = participantForm.Id;
            this.FirstName = participantForm.FirstName;
            this.LastName = participantForm.LastName;
            this.EmailAddress = participantForm.EmailAddress;
            this.PhoneNumber1 = participantForm.PhoneNumber1;
            this.PhoneExtension1 = participantForm.PhoneExtension1;
            this.PrimaryCity = participantForm.PrimaryCity;
            this.ClaimReported = participantForm.ClaimReported;
            this.IsIncludedInClaim = !participantForm.IsExcludedFromClaim;
            this.DateAdded = participantForm.DateAdded.ToLocalTime();
            this.RowVersion = participantForm.RowVersion;
			this.Attended = participantForm.Attended;
        }
        #endregion
    }
}