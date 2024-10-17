using System;
using System.Linq;
using CJG.Core.Interfaces.Service;

namespace CJG.Application.Services.PartialUpdate
{
    internal class ReferenceUpdater<TEntity, TRefType> : PartialUpdaterBase<TEntity> 
        where TEntity : class
        where TRefType : class
    {
        private readonly Action<TEntity, TRefType> _setValue;
        private readonly Func<int, TRefType> _getRefByNum;
        private readonly Func<string, TRefType> _getRefByText;
        private readonly Func<string, string, TRefType> _getRefByKeys;
        private readonly Func<int, string> _convertNum;
        private readonly Func<string, string> _convertText;
        private readonly Func<string, string, string> _convertKeysToText;

        public ReferenceUpdater(string systemNoteText, 
            Action<TEntity, TRefType> setValue, 
            Func<int, TRefType> getRefByNum, 
            Func<TEntity, string> convertEntityToText, 
            Func<int, string> convertNum, 
            Action<TEntity> ensureExists = null,
            bool allowEmptyValue = true) : base(systemNoteText, convertEntityToText, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
            _getRefByNum = getRefByNum;
            _convertNum = convertNum;
        }

        public ReferenceUpdater(string systemNoteText, 
            Action<TEntity, TRefType> setValue,
            Func<string, TRefType> getRefByText, 
            Func<TEntity, string> convertEntityToText, 
            Func<string, string> convertText, 
            Action<TEntity> ensureExists = null,
            bool allowEmptyValue = true) : base(systemNoteText, convertEntityToText, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
            _getRefByText = getRefByText;
            _convertText = convertText;
        }

        public ReferenceUpdater(string systemNoteText, 
            Action<TEntity, TRefType> setValue,
            Func<string, string, TRefType> getRefByKeys, 
            Func<TEntity, string> convertEntityToText, 
            Func<string, string, string> convertText, 
            Action<TEntity> ensureExists = null,
            bool allowEmptyValue = true) : base(systemNoteText, convertEntityToText, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
            _getRefByKeys = getRefByKeys;
            _convertKeysToText = convertText;
        }

        public override void UpdateValue(TEntity entity, string[] arr)
        {
            TRefType refValue = null;

            if (HasNumberKey)
            {
                var numKeyValue = Convert.ToInt32(arr[0]);
                if (numKeyValue != PartialEntityUpdateConstants.NoneKey)
                {
                    refValue = _getRefByNum(numKeyValue);
                }
            }
            else if (_getRefByText != null)
            {
                if (arr[0] != PartialEntityUpdateConstants.NoneKey.ToString())
                {
                    refValue = _getRefByText(arr[0]);
                }
            }
            else if (_getRefByKeys != null)
            {
                if (arr[0] != PartialEntityUpdateConstants.NoneKey.ToString() && arr[1] != PartialEntityUpdateConstants.NoneKey.ToString())
                {
                    refValue = _getRefByKeys(arr[0], arr[1]);
                }
            }
            else
            {
                throw new ApplicationException("Can't find function to read selected value");
            }
            
            _setValue(entity, refValue);
        }

        private bool HasNumberKey => _getRefByNum != null;

        public override string GetNewValueText(string[] arr)
        {
            if (arr != null && arr.Any())
            {
                return HasNumberKey
                    ? _convertNum(int.Parse(arr[0]))
                    : (_convertText != null ? _convertText(arr[0]) : (_convertKeysToText != null ? _convertKeysToText(arr[0], arr[1]) : arr[0]));
            }

            return base.GetNewValueText(arr);
        }
    }
}