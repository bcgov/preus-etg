using System;

namespace CJG.Application.Services.PartialUpdate
{
    internal class DateUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, DateTime?> _setValue;

        public DateUpdater(string systemNoteText, Action<TEntity, DateTime?> setValue, Func<TEntity, string> convertEntityToText, Action<TEntity> ensureExists = null) : base(systemNoteText, convertEntityToText, ensureExists)
        {
            _setValue = setValue;
        }

        public override void UpdateValue(TEntity app, string[] arr)
        {
            // working on the assumption that the date being passed-in is midnight, specify as local and then convert to UTC to ensure that the correct date is saved
            _setValue(app, DateTime.SpecifyKind(DateTime.Parse(arr[0]), DateTimeKind.Local).ToUniversalTime());
        }
    }
}