using System;
using System.Threading;
using Godot;

/*
 * A Logger that can be accessed from anywhere, used as a replacement for default GODOT logging 
 * To allow better customization in logs
 */

public static class Logger
{
    public static void DebugInfo(string message, params object[] parameters)
    {
        if (!OS.IsDebugBuild()) return;
        Info(message, parameters);
    }
    public static void Info(string message, params object[] parameters)
    {
        Log(ELogSeverity.Info, message, parameters);
    }
    public static void DebugWarning(string message, params object[] parameters)
    {
        if (!OS.IsDebugBuild()) return;
        Warning(message, parameters);
    }
    public static void Warning(string message, params object[] parameters)
    {
        Log(ELogSeverity.Warning, message, parameters);
    }
    public static void DebugError(string message, params object[] parameters)
    {
        if (!OS.IsDebugBuild()) return;
        Error(message, parameters);
    }
    public static void Error(string message, params object[] parameters)
    {
        Log(ELogSeverity.Error, message, parameters);
    }
    
    ///Throws an application exception in debug
    public static void Fatal(string message, params object[] parameters)
    {
        string msg = Log(ELogSeverity.Fatal, message, parameters);
        if (OS.IsDebugBuild())
        {
            throw new ApplicationException(msg);
        }
    }

    ///Throws a Fatal log which includes assert if the condition is false <br/>
    ///Returns true if the condition caused an exception
    public static bool Assert(bool condition, string message,params object[] parameters)
    {
        if (!condition)
        {
            Fatal(message, parameters);
            return true;
        }

        return false;
    }

    private static string Log(ELogSeverity severity, string message, params object[] parameters)
    {
        LogMessage msg = new LogMessage(severity, message, parameters);
        message = msg.Parse();
        GD.Print(message);
        return message;
    }
    
}