using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Application.Services.PartialUpdate
{
    internal class RiskClassificationFields
    {
        public RiskClassification RiskClassification { get; set; }
    }

    internal class RiskClassificationUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : GrantApplication
    {
        private readonly Action<TEntity, RiskClassificationFields> _setValue;
        private readonly Func<int, RiskClassification> _getRiskClassificationById;
        
        public RiskClassificationUpdater(string systemNoteText,
            Action<TEntity, RiskClassificationFields> setValue,
            Func<int, RiskClassification> getRiskClassificationById,
            Func<TEntity, string> convertEntityToText = null,
            Action<TEntity> ensureExists = null,
            bool allowEmptyValue = true) : base(systemNoteText, convertEntityToText, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
            _getRiskClassificationById = getRiskClassificationById;

            ConvertEntityToText = ConvertApplicationToText;
        }

        public override void UpdateValue(TEntity entity, string[] arr)
        {
            var fields = ConvertArrToFields(arr);

            Validate(fields);

            _setValue(entity, fields);
        }

        private void Validate(RiskClassificationFields fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }
        }

        private RiskClassificationFields ConvertArrToFields(string[] arr)
        {
            var riskClassificationId = int.Parse(arr[0]);

            return new RiskClassificationFields()
            {
                RiskClassification = _getRiskClassificationById(riskClassificationId)
            };
        }

        private string ConvertFieldsToText(RiskClassificationFields fields)
        {
            if (fields?.RiskClassification == null) return "None";

            SystemNoteText = PartialEntityUpdateConstants.GrantApplicationRiskClassification;

            var result = fields.RiskClassification.Caption;

            return result;
        }

        public string ConvertApplicationToText(GrantApplication app)
        {
            return ConvertFieldsToText(ConvertApplicationToFields(app));
        }

        private RiskClassificationFields ConvertApplicationToFields(GrantApplication app)
        {
            return new RiskClassificationFields()
            {
                RiskClassification = app.RiskClassification
            };
        }

        public override string GetNewValueText(string[] arr)
        {
            return ConvertFieldsToText(ConvertArrToFields(arr));
        }
    }
}
