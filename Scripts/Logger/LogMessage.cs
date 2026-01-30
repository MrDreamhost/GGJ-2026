using System;
using System.Diagnostics;

/*
 *  An object used by the logger that contains all info for a singular parseable log message
 */
public class LogMessage
{
    private readonly string _message;
    private readonly DateTime _timeStamp;
    private readonly ELogSeverity _severity;
    private readonly StackFrame _stackFrame;
    private readonly object[] _parameters;

    public LogMessage(ELogSeverity severity, string message, Object[] parameters)
    {
        _message = message;
        _timeStamp = DateTime.UtcNow;
        _severity = severity;
        _stackFrame = GetFirstNonLoggerStackFrame();
        _parameters = parameters;
    }

    ///Will find the first stack frame that is not part of the logger.
    ///Can return null
    private static StackFrame GetFirstNonLoggerStackFrame()
    {
        var stackTrace = new StackTrace();
        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            var frame = stackTrace.GetFrame(i);
            var declaringType = frame?.GetMethod()?.DeclaringType?.ToString();
            if (declaringType != null && !declaringType.Contains("Logger") && !declaringType.Contains("LogMessage"))
            {
                return frame;
            }
        }
        return null;
    }

    ///Parses all info in the object to a single string that can be used to print out the log
    public string Parse()
    {
        string message = string.Format(_message, _parameters);
        return string.Format("{0} [{1}] | {2}::{3} | {4}",
            _timeStamp,
            _severity,
            _stackFrame?.GetMethod()?.DeclaringType,
            _stackFrame?.GetMethod()?.Name,
            message);
    }
}