using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CJG.Application.Services.PartialUpdate
{
    internal class EmailUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, string> _setValue;

        private static readonly Regex EmailRegExPattern = new Regex(@"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$"); //NOSONAR

        public EmailUpdater(string systemNoteText, Action<TEntity, string> setValue, Func<TEntity, 
            string> convertEntityToText, Action<TEntity> ensureExists = null, bool allowEmptyValue = true) : base(systemNoteText, convertEntityToText, ensureExists, allowEmptyValue)
        {
            _setValue = setValue;
        }

        public override void UpdateValue(TEntity app, string[] arr)
        {
            if (arr != null)
            {
                if (arr.Contains(string.Empty))
                {
                    throw new ArgumentException($"Email address is required");
                }
                else if (arr.Any() && !IsValidEmail(arr[0]))
                {
                    throw new ArgumentException($"Invalid email address: {arr[0]}");
                }
            }

            _setValue(app, arr?[0]);
        }

        public static bool IsValidEmail(string rawEmailText)
        {
            return EmailRegExPattern.Match(rawEmailText.Trim()).Success;
        }
    }
}