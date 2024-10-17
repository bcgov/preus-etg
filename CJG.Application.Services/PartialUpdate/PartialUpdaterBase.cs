using System;
using System.Linq;

namespace CJG.Application.Services.PartialUpdate
{
    internal abstract class PartialUpdaterBase<TEntity> : IPartialUpdaterBase
        where TEntity : class
    {
        public Func<TEntity, string> ConvertEntityToText;
        private readonly Action<TEntity> _ensureExists;

        protected PartialUpdaterBase(string systemNoteText, Func<TEntity, string> convertEntityToText = null, Action<TEntity> ensureExists = null, bool allowEmptyValue = true)
        {
            ConvertEntityToText = convertEntityToText;
            _ensureExists = ensureExists;
            SystemNoteText = systemNoteText;
            AllowEmptyValue = allowEmptyValue;
        }

        public bool AllowUpdateSystemNotes { get; set; } = true;

        public string SystemNoteText { get; set; }

        public virtual string GetSystemNoteText()
        {
            return SystemNoteText;
        }

        public void UpdateNewValue(TEntity entity, string[] arr)
        {
            _ensureExists?.Invoke(entity);
            UpdateValue(entity, arr);
        }

        public abstract void UpdateValue(TEntity entity, string[] arr);

        public void UpdateNewValue(object entity, string[] arr)
        {
            if (!(arr.Any() && !arr.Contains(string.Empty) && arr[0] != "-1") && !AllowEmptyValue)
            {
                throw new ApplicationException($"{SystemNoteText} is required");
            }

            UpdateNewValue((TEntity) entity, arr);
        }

        public virtual string GetExistingValueText(TEntity entity)
        {
            if (ConvertEntityToText == null)
            {
                throw new ApplicationException("Can't find reference to getValue function");
            }

            return ConvertEntityToText(entity);
        }

        public virtual string GetNewValueText(string[] arr)
        {
            return arr == null || !arr.Any() ? null : arr[0];
        }

        public string GetExistingValueText(object entity)
        {
            return GetExistingValueText((TEntity)entity);
        }

        public bool AllowEmptyValue { get; set; } = true;
    }

    internal interface IPartialUpdaterBase
    {
        string SystemNoteText { get; }
        bool AllowUpdateSystemNotes { get; set; } 
        string GetExistingValueText(object entity);
        void UpdateNewValue(object entity, string[] arr);
        string GetNewValueText(string[] arr);
        bool AllowEmptyValue { get; set; }
    }
}