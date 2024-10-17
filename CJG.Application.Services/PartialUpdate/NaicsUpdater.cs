using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Application.Services.PartialUpdate
{
    internal class NaicsUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, NaIndustryClassificationSystem> _setValue;
        private readonly Func<int, IEnumerable<NaIndustryClassificationSystem>> _getNaicsById;

        public NaicsUpdater(string systemNoteText, Action<TEntity, NaIndustryClassificationSystem> setValue,
            Func<TEntity, string> convertEntityToText, Func<int, IEnumerable<NaIndustryClassificationSystem>> getNaicsById,
            Action<TEntity> ensureExists = null) : base(systemNoteText, convertEntityToText, ensureExists)
        {
            _setValue = setValue;
            _getNaicsById = getNaicsById;
        }

        public override void UpdateValue(TEntity entity, string[] arr)
        {
            ValidateNaicsFields(arr);

            var naicsNodes = ConvertArrayToNaics(arr)?.OrderBy(x=>x.Level).ToList();
            
            _setValue(entity, naicsNodes?.LastOrDefault());
        }

        private static void ValidateNaicsFields(IReadOnlyList<string> arr)
        {
            string invalidField = null;
            if (arr == null || !arr.Any() || string.IsNullOrWhiteSpace(arr[0]) || arr[0] == PartialEntityUpdateConstants.NoneKey.ToString() || arr[0] == "null")
            {
                invalidField = "Naics Code Part 1";
            }
            else if (arr.Count > 0 && (string.IsNullOrWhiteSpace(arr[1]) || arr[1] == PartialEntityUpdateConstants.NoneKey.ToString() || arr[1] == "null") )
            {
                invalidField = "Naics Code Part 2";
            }
            else if (arr.Count > 1 && (string.IsNullOrWhiteSpace(arr[2]) || arr[2] == PartialEntityUpdateConstants.NoneKey.ToString() || arr[2] == "null"))
            {
                invalidField = "Naics Code Part 3";
            }

            if (invalidField != null)
            {
                throw new ApplicationException($"{invalidField} is required");
            }
        }

        public override string GetNewValueText(string[] arr)
        {
            return ConvertArrayToFormattedText(arr);
        }

        private string ConvertArrayToFormattedText(string[] arr)
        {
            return ConvertNaicsToFormattedText(ConvertArrayToNaics(arr)?.OrderBy(x=>x.Level).ToList());
        }

        internal IEnumerable<NaIndustryClassificationSystem> ConvertArrayToNaics(string[] arr)
        {
            if (arr.Length > 2 && arr[2] != null)
            {
                return _getNaicsById(Convert.ToInt32(arr[2]));
            }

            if (arr.Length > 1 && arr[1] != null)
            {
                return _getNaicsById(Convert.ToInt32(arr[1]));
            }

            if (arr.Length > 0 && arr[0] != null)
            {
                return _getNaicsById(Convert.ToInt32(arr[0]));
            }

            return null;
        }

        internal static string ConvertNaicsToFormattedText(IList<NaIndustryClassificationSystem> naics)
        {
            if (naics == null || !naics.Any())
            {
                return "None";
            }

            return naics.Last().ToString();
            
        }
    }
}