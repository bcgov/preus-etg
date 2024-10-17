using System;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Application.Services.PartialUpdate
{
    internal class AdditionalFundingFields
    {
        public bool HasRequestedAdditionalFunding { get; set; }
        public string AdditionalFundingDescription { get; set; }
    }

    internal class AdditionalFundingUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : GrantApplication
    {
        private readonly Action<TEntity, AdditionalFundingFields> _setValue;

        public AdditionalFundingUpdater(string systemNoteText,
            Action<TEntity, AdditionalFundingFields> setValue,
            Action<TEntity> ensureExists = null) : base(systemNoteText, 
                null, ensureExists)
        {
            _setValue = setValue;
            ConvertEntityToText = ConvertApplicationToText;
        }

        public override void UpdateValue(TEntity entity, string[] arr)
        {
            var additionalFundingFields = ConvertArrayToAdditionalFundingFields(arr);

            if (additionalFundingFields == null)
                throw new ApplicationException("Can't convert selected values to AdditiondlFundingFields");

            var skillFocusFields = ConvertArrayToAdditionalFundingFields(arr);

            _setValue(entity, skillFocusFields);
        }

        public override string GetNewValueText(string[] arr)
        {
            return ConvertAdditionalFundingFieldsToText(ConvertArrayToAdditionalFundingFields(arr));
        }

        private AdditionalFundingFields ConvertArrayToAdditionalFundingFields(string[] arr)
        {
            if (arr == null || !arr.Any()) return null;

            var additionalFundingFields = new AdditionalFundingFields();

            int hasRequestedAdditionalFunding;
            if (arr.Length > 0 && int.TryParse(arr[0], out hasRequestedAdditionalFunding))
            {
                additionalFundingFields.HasRequestedAdditionalFunding = hasRequestedAdditionalFunding == 1;

                if (arr.Length > 1 && !string.IsNullOrWhiteSpace( arr[1] ) )
                {
                    additionalFundingFields.AdditionalFundingDescription = arr[1];
                }
            }

            return additionalFundingFields;
        }

        public string ConvertApplicationToText(GrantApplication app)
        {
            return ConvertAdditionalFundingFieldsToText(ConvertApplicationToAdditionalFundingFields(app));
        }

        private string ConvertAdditionalFundingFieldsToText(AdditionalFundingFields additionalFundingFields)
        {
            SystemNoteText = PartialEntityUpdateConstants.TrainingProgramHasRequestedAdditionalFunding;

            var result = additionalFundingFields.HasRequestedAdditionalFunding ? "Yes" : "No";

            if (additionalFundingFields.AdditionalFundingDescription != null)
            {
                result += "\\" + additionalFundingFields.AdditionalFundingDescription;
            }

            return result;
        }

        private AdditionalFundingFields ConvertApplicationToAdditionalFundingFields(GrantApplication grantApplication)
        {
            return new AdditionalFundingFields()
            {
                HasRequestedAdditionalFunding = grantApplication.TrainingPrograms.FirstOrDefault().HasRequestedAdditionalFunding,
                AdditionalFundingDescription = grantApplication.TrainingPrograms.FirstOrDefault().DescriptionOfFundingRequested
            };
        }
    }
}