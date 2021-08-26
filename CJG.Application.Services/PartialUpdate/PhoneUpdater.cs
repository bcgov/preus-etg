using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CJG.Application.Services.PartialUpdate
{
    internal class PhoneUpdater<TEntity> : PartialUpdaterBase<TEntity> where TEntity : class
    {
        private readonly Action<TEntity, string> _setPhoneNumberValue;
        private readonly Action<TEntity, int> _setPhoneAreaCode;
        private readonly Action<TEntity, int> _setPhoneExchange;
        private readonly Action<TEntity, int> _setPhoneNumber;
        private readonly Action<TEntity, string> _setPhoneExtension;
        private readonly Func<TEntity, string> _getPhoneNumber;
        private readonly Func<TEntity, string> _getPhoneAreaCode;
        private readonly Func<TEntity, string> _getPhoneExchange;
        private readonly Func<TEntity, string> _getPhoneExtension;
        private readonly Action<TEntity> _ensureAction;
        private readonly Action<TEntity, int?> _setPhoneNumExtension;

        private static readonly Regex PhoneRegExPattern = new Regex(@"^([\\(])?(?<areaCode>\d{3})([\\)])?(-|\W)(?<part1>\d{3})-(?<part2>\d{4})(\s(ext(\.)?\W)?(?<ext>\d{1,4}))?$"); //NOSONAR
        

        public PhoneUpdater(string systemNoteText, Action<TEntity, string> setPhoneNumberValue, Action<TEntity, string> setPhoneExtension, 
            Func<TEntity, string> getPhoneNumber, Func<TEntity, string> getPhoneExt = null, bool allowEmptyValue = true) : base(systemNoteText, null, null, allowEmptyValue)
        {
            _setPhoneNumberValue = setPhoneNumberValue;
            _setPhoneExtension = setPhoneExtension;
            _getPhoneNumber = getPhoneNumber;
            _getPhoneExtension = getPhoneExt;
        }

        public PhoneUpdater(string systemNoteText, Action<TEntity, int> setPhoneAreaCode, Action<TEntity, int> setPhoneExchange,
            Action<TEntity, int> setPhoneNumber,
            Action<TEntity, int?> setPhoneExtension,
            Func<TEntity, string> getPhoneNumber, Func<TEntity, string> getPhoneExtension = null,
            Action<TEntity> ensureAction = null,
            bool allowEmptyValue = true) : base(systemNoteText, null, null, allowEmptyValue)
        {
            _setPhoneAreaCode = setPhoneAreaCode;
            _setPhoneExchange = setPhoneExchange;
            _setPhoneNumber = setPhoneNumber;
            _setPhoneNumExtension = setPhoneExtension;
            _getPhoneNumber = getPhoneNumber;
            _getPhoneExtension = getPhoneExtension;
            _ensureAction = ensureAction;
        }

        public PhoneUpdater(
            string systemNoteText, 
            Action<TEntity, int> setPhoneAreaCode, 
            Action<TEntity, int> setPhoneExchange,
            Action<TEntity, int> setPhoneNumber,
            Action<TEntity, int?> setPhoneExtension,
            Func<TEntity, string> getPhoneAreaCode,
            Func<TEntity, string> getPhoneExchange,
            Func<TEntity, string> getPhoneNumber, 
            Func<TEntity, string> getPhoneExtension = null,
            Action<TEntity> ensureAction = null,
            bool allowEmptyValue = true) : base(systemNoteText, null, null, allowEmptyValue)
        {
            _setPhoneAreaCode = setPhoneAreaCode;
            _setPhoneExchange = setPhoneExchange;
            _setPhoneNumber = setPhoneNumber;
            _setPhoneNumExtension = setPhoneExtension;
            _getPhoneAreaCode = getPhoneAreaCode;
            _getPhoneExchange = getPhoneExchange;
            _getPhoneNumber = getPhoneNumber;
            _getPhoneExtension = getPhoneExtension;
            _ensureAction = ensureAction;
        }

        public override void UpdateValue(TEntity app, string[] arr)
        {
            string phoneExtension = null;

            if (_setPhoneNumberValue != null)
            {
                string phoneNumber = null;

                if (arr != null)
                {
                    if (arr.Contains(string.Empty))
                    {
                        throw new ArgumentException(
                            "Phone number is required");

                    }
                    else if (arr.Any() && !TryParseTwoPartsPhoneNumber(arr[0], out phoneNumber, out phoneExtension))
                    {
                        throw new ArgumentException(
                            "Invalid format. Expected phone number format: (123) 123-1234 ext. 1234");
                    }
                }

                _setPhoneNumberValue(app, phoneNumber);
                _setPhoneExtension(app, phoneExtension);
            }
            else if (_setPhoneAreaCode != null && _setPhoneExchange != null && _setPhoneNumber != null)
            {
                string areaCode = null;
                string part1 = null;
                string part2 = null;
                if (arr != null && arr.Any() &&
                    !TryParseFourPartsPhoneNumber(arr[0], out areaCode, out part1, out part2, out phoneExtension))
                    throw new ArgumentException(
                        "Not valid format. Expected phone number should contain only numbers: (123) 123-1234 {1234}");

                _ensureAction?.Invoke(app);

                _setPhoneAreaCode(app, int.Parse(areaCode));
                _setPhoneExchange(app, int.Parse(part1));
                _setPhoneNumber(app, int.Parse(part2));
                _setPhoneNumExtension(app, string.IsNullOrWhiteSpace(phoneExtension) ? (int?)null: int.Parse(phoneExtension));
            }
            else
            {
                throw new InvalidOperationException("Can't find update handler for parts of phone number");
            }
        
            
        }

        public override string GetExistingValueText(TEntity app)
        {
            var ext = _getPhoneExtension?.Invoke(app);
            return _getPhoneNumber(app) + (string.IsNullOrWhiteSpace(ext) ? null : $" ext {ext}");
        }

        public static bool TryParseTwoPartsPhoneNumber(string rawPhoneText, out string phoneNumber, out string phoneExtension)
        {
            phoneNumber = null;
            string areaCode;
            string part1;
            string part2;
            var isParcedSuccess = TryParseFourPartsPhoneNumber(rawPhoneText, out areaCode, out part1, out part2, out phoneExtension);
            if (isParcedSuccess)
            {
                phoneNumber = $"({areaCode}) {part1}-{part2}";
            }

            return isParcedSuccess;
        }

        public static bool TryParseFourPartsPhoneNumber(string rawPhoneText, out string areaCode, out string part1, out string part2, out string extension)
        {
            extension = null;
            areaCode = null;
            part1 = null;
            part2 = null;

            var match = PhoneRegExPattern.Match(rawPhoneText.Trim());
            if (!match.Success || !match.Groups["areaCode"].Success || !match.Groups["part1"].Success || !match.Groups["part2"].Success)
                return false;

            areaCode = match.Groups["areaCode"].Value;
            part1 =match.Groups["part1"].Value;
            part2 =match.Groups["part2"].Value;

            if (match.Groups["ext"].Success)
            {
                extension = match.Groups["ext"].Value;
            }

            return true;
        }

    }
}