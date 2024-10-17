using System;
using System.Linq;
using CJG.Core.Interfaces.Service;

namespace CJG.Application.Services.PartialUpdate
{
    internal class FlagUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, bool> _setValue;
        private readonly Func<TEntity, bool> _getValue;

        public FlagUpdater(string systemNoteText, Action<TEntity, bool> setValue, Func<TEntity, bool> getValue) : base(systemNoteText)
        {
            _setValue = setValue;
            _getValue = getValue;
        }

        public override void UpdateValue(TEntity app, string[] arr)
        {
            _setValue(app, Convert.ToBoolean(Convert.ToInt32(arr[0])));
        }

        public override string GetExistingValueText(TEntity app)
        {
            return _getValue(app) ? PartialEntityUpdateConstants.Yes : PartialEntityUpdateConstants.No;
        }

        public override string GetNewValueText(string[] arr)
        {
            return arr != null && arr.Any()
                ? (Convert.ToBoolean(Convert.ToInt32(arr[0]))
                    ? PartialEntityUpdateConstants.Yes
                    : PartialEntityUpdateConstants.No)
                : base.GetNewValueText(arr);
        }
    }
}