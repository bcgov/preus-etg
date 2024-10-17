using System;
using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace CJG.Web.External.Helpers
{
    public sealed class ListLoggerTarget : TargetWithLayout
    {
        private readonly IList<string> _list;

        public ListLoggerTarget(IList<string> list)
        {
            _list = list;
            Layout = "${longdate}|${level:uppercase=true}|${message}";
        }

        protected override void Write(LogEventInfo logEvent)
        {
            _list.Add(Layout.Render(logEvent));
        }
    }

    public class DisposableListLogger : IDisposable
    {
        private readonly LoggingConfiguration _loggingConfiguration;
        private readonly LoggingRule _loggingRule;

        public DisposableListLogger(IList<string> list, LoggingConfiguration loggingConfiguration)
        {
            _loggingConfiguration = loggingConfiguration;
            var logTarget = new ListLoggerTarget(list);
            _loggingRule = new LoggingRule("*", LogLevel.Debug, logTarget);
            _loggingConfiguration.AddTarget("listTarget", logTarget);
            _loggingConfiguration.LoggingRules.Add(_loggingRule);
            _loggingConfiguration.Reload();
        }

        public void Dispose()
        {
            if (_loggingConfiguration == null) return;
            _loggingConfiguration.LoggingRules.Remove(_loggingRule);
            _loggingConfiguration.RemoveTarget("listTarget");
            _loggingConfiguration.Reload();
        }
    }
}