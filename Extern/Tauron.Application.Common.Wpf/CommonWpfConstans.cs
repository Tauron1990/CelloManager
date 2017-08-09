using System;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application
{
    public static class CommonWpfConstans
    {
        public const string CommonCategory = "Tauron.Application.Common.Wpf";
        public const string CommonExceptionPolicy = "Tauron.Application.Common.Wpf.Policy";

        [StringFormatMethod("format")]
        public static void LogCommon(bool isError, [NotNull] string format, [NotNull] [ItemNotNull] params object[] parms)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (parms == null) throw new ArgumentNullException(nameof(parms));
            var realMessage = parms.Length == 0 ? format : string.Format(format, parms);
            LogManager.GetLogger(CommonCategory, typeof(CommonWpfConstans)).Log(isError ? LogLevel.Error : LogLevel.Warn, realMessage, parms);
        }
    }
}