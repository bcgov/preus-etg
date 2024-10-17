using System;

namespace CJG.Application.Services.PartialUpdate
{
    internal class TextUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, string> _setValue;

        public TextUpdater(string systemNoteText, Action<TEntity, string> setValue, Func<TEntity, string> convertEntityToText, Action<TEntity> ensureExists = null, bool allowEmptyValue = true) : base(systemNoteText, convertEntityToText, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
        }
        
        public override void UpdateValue(TEntity app, string[] arr)
        {
            _setValue(app, arr[0]);
        }
    }
}