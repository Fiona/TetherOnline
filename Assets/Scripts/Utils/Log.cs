using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TetherOnline
{

#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
    static class EditorLogSetter
    {
        private static CustomLogHandler customLogHandler;
        static EditorLogSetter()
        {
            customLogHandler = new CustomLogHandler();
        }
    }
#endif


    public static class Log
    {
        public static void Message(string message, string context = "???")
        {
            Debug.LogFormat("{0}", ConstructMessage(message, context), $"[{context}]");
        }

        public static void Success(string message, string context = "???")
        {
            Debug.LogFormat("{0}", ConstructMessage(message, colour:"green"), $"[{context}]");
        }

        public static void Warning(string message, string context = "???")
        {
            Debug.LogWarningFormat("{0}", ConstructMessage(message, colour:"orange"), $"[{context}]");
        }

        public static void Error(string message, string context = "???")
        {
            Debug.LogErrorFormat("{0}", ConstructMessage(message, colour:"red"), $"[{context}]");
        }

        private static string ConstructMessage(string message, string colour = "")
        {
            var rtMessage = new []{"", ""};
#if UNITY_EDITOR
            if(colour != "")
            {
                rtMessage[0] = $"<color={colour}>";
                rtMessage[1] = "</color>";
            }
#endif
            return $"{rtMessage[0]}{message}{rtMessage[1]}";
        }
    }

    public class CustomLogHandler : ILogHandler
    {
        private ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;

        public CustomLogHandler()
        {
            Debug.unityLogger.logHandler = this;
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            var messageFormat = ConstructMessageFormat();
            if(args.Length == 1)
            {
                args = new[] { args[0], GuessContext() };
            }
            defaultLogHandler.LogFormat(logType, context, messageFormat, args);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            defaultLogHandler.LogException(exception, context);
        }

        private static string ConstructMessageFormat()
        {
            var rtTime = new []{"", ""};
#if UNITY_EDITOR
            rtTime[0] = "<b>";
            rtTime[1] = "</b>";
#endif
            var time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            return $"{rtTime[0]}[{time}]{rtTime[1]} {{1,10}} {{0}}";
        }

        private string GuessContext()
        {
            var finalNamespace = "";
            var reflectedType = new StackTrace().GetFrame(4).GetMethod().ReflectedType;
            var capsNamespace = reflectedType == null ? "" : reflectedType.FullName.ToUpper();
            if(capsNamespace.StartsWith("MIRROR") || capsNamespace.StartsWith("TELEPATHY"))
                finalNamespace = "MIRROR";
            if(capsNamespace.StartsWith("MONKE"))
                finalNamespace = "MONKE";
            return finalNamespace == "" ? "" : $"[{finalNamespace}]";
        }
    }
}