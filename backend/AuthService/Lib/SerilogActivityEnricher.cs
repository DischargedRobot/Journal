using System.Diagnostics;

using Serilog.Core;
using Serilog.Events;

namespace AuthService.Lib
{
    public class SerilogActivityEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            Activity? activity = Activity.Current;
            if (activity == null)
                return;

            if (!string.IsNullOrEmpty(activity.TraceId.ToString()))
            {
                LogEventProperty traceProp = propertyFactory.CreateProperty(
                    "TraceId",
                    activity.TraceId.ToString()
                );
                logEvent.AddPropertyIfAbsent(traceProp);
            }

            if (!string.IsNullOrEmpty(activity.SpanId.ToString()))
            {
                LogEventProperty spanProp = propertyFactory.CreateProperty(
                    "SpanId",
                    activity.SpanId.ToString()
                );
                logEvent.AddPropertyIfAbsent(spanProp);
            }

            if (!activity.TraceId.Equals(default) && !activity.SpanId.Equals(default))
            {
                string sampled = activity.ActivityTraceFlags.HasFlag(ActivityTraceFlags.Recorded) ? "01" : "00";
                string traceParent = $"00-{activity.TraceId}-{activity.SpanId}-{sampled}";
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceParent", traceParent));
            }
        }
    }
}
