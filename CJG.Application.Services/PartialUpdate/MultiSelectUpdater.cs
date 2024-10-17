using System;
using System.Linq;

namespace CJG.Application.Services.PartialUpdate
{
    internal class MultiSelectUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, int[]> _setValue;
        private readonly Func<TEntity, string[]> _getValue;
        private readonly Func<int, string> _convert;

        public MultiSelectUpdater(string systemNoteText, Action<TEntity, int[]> setValue, 
            Func<TEntity, string[]> getValue, Func<int, string> convert, bool allowEmptyValue = true) : base(systemNoteText, null, null, allowEmptyValue)
        {
            _setValue = setValue;
            _getValue = getValue;
            _convert = convert;
        }
        
        public override void UpdateValue(TEntity app, string[] arr)
        {
            _setValue(app, arr?.Select(int.Parse).ToArray());
        }

        public override string GetExistingValueText(TEntity app)
        {
            var valueArr = _getValue(app);
            return valueArr != null ? string.Join(",", valueArr.Select(x => $"[{x}]")) : null;
        }

        public override string GetNewValueText(string[] arr)
        {
            return arr == null || !arr.Any()
                ? null
                : string.Join(",", arr.Select(int.Parse).Select(x => $"[{_convert(x)}]"));
        }
    }
}