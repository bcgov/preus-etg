using System;

namespace CJG.Application.Services.PartialUpdate
{
    internal class IntUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, int?> _setValue;

        public IntUpdater(string systemNoteText, Action<TEntity, int?> setValue, Func<TEntity, string> convertEntityToText, Action<TEntity> ensureExists = null) : base(systemNoteText, convertEntityToText, ensureExists)
        {
            _setValue = setValue;
        }
        
        public override void UpdateValue(TEntity app, string[] arr)
        {
            _setValue(app, int.Parse(arr[0]));
        }
    }
}