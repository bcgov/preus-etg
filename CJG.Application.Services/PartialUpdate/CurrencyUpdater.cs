using System;

namespace CJG.Application.Services.PartialUpdate
{
    internal class CurrencyUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, decimal?> _setValue;

        public CurrencyUpdater(string systemNoteText, Action<TEntity, decimal?> setValue, Func<TEntity, string> convertEntityToText, Action<TEntity> ensureExists = null) : base(systemNoteText, convertEntityToText, ensureExists)
        {
            _setValue = setValue;
        }

        public override void UpdateValue(TEntity app, string[] arr)
        {
            // need to remove any currency formatting
            arr[0] = decimal.Parse(arr[0], System.Globalization.NumberStyles.Currency).ToString("g");
            _setValue(app, decimal.Parse(arr[0]));
        }
    }
}