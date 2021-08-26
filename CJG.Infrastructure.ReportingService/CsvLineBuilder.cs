using System.Text;

namespace CJG.Infrastructure.ReportingService
{
    public class CsvLineBuilder
    {
        private readonly char[] _csvTokens = {'\"', ',', '\n', '\r'};
        private readonly StringBuilder _stringBuilder;
        private int _columnCount;

        public CsvLineBuilder(StringBuilder stringBuilder = null)
        {
            _stringBuilder = stringBuilder ?? new StringBuilder();
            _columnCount = 0;
        }

        public CsvLineBuilder AppendColumn(string columnValue)
        {
            if (_stringBuilder.Length > 0 || _columnCount > 0)
            {
                _stringBuilder.Append(',');
            }

            if (!string.IsNullOrEmpty(columnValue))
            {
                _stringBuilder.Append(EncodeValue(columnValue));
            }

            _columnCount++;

            return this;
        }
        
        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        private string EncodeValue(string csvValue)
        {
            return csvValue.IndexOfAny(_csvTokens) >= 0
                ? $"\"{csvValue.Replace("\"", "\"\"")}\""
                : csvValue;
        }
    }
}