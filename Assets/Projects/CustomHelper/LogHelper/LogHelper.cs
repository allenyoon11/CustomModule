using System;
using System.Linq;
using UnityEngine;

namespace neuroears.allen.utils
{

    public static class Log
    {
        public enum LogType
        {
            LOG,INFO,WARN,ERROR,FAIL,SUCCESS,TODO,
        }
        public static void l(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"{message}");
#else
            if (!editorOnly)
            {
                Debug.Log($"{message}");
            }
#endif
        }

        public static void l(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"{TypeToString(type)} {message}");
#else
            if (!editorOnly)
            {
                Debug.Log($"{TypeToString(type)} {message}");
            }
#endif
        }
        public static void l(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"{TypeToString(type)} {LogTypeToString(logType)} {message}");
#else
            if (!editorOnly)
            {
                Debug.Log($"{TypeToString(type)} {LogTypeToString(logType)} {message}");
            }
#endif
        }

        public static void r(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=red>{message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=red>{message}</color>");
            }
#endif
        }
        public static void r(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=red>{TypeToString(type)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=red>{TypeToString(type)} {message}</color>");
            }
#endif
        }
        public static void r(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=red>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=red>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
            }
#endif
        }
        public static void b(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=#00BFFF>{message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=#00BFFF>{message}</color>");
            }
#endif
        }
        public static void b(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=#00BFFF>{TypeToString(type)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=#00BFFF>{TypeToString(type)} {message}</color>");
            }
#endif
        }
        public static void b(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=#00BFFF>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=#00BFFF>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
            }
#endif
        }
        public static void y(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=yellow>{message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=yellow>{message}</color>");
            }
#endif
        }
        public static void y(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>{TypeToString(type)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=yellow>{TypeToString(type)} {message}</color>");
            }
#endif
        }
        public static void y(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=yellow>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
            }
#endif
        }
        public static void g(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR

            Debug.Log($"<color=#00FF00>{message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=#00FF00>{message}</color>");
            }
#endif
        }
        public static void g(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#00FF00>{TypeToString(type)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=#00FF00>{TypeToString(type)} {message}</color>");
            }
#endif
        }        
        public static void g(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=#00FF00>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=#00FF00>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
            }
#endif
        }
        public static void o(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=orange>{message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=orange>{message}</color>");
            }
#endif
        }
        public static void o(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=orange>{TypeToString(type)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=orange>{TypeToString(type)} {message}</color>");
            }
#endif
        }
        public static void o(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.Log($"<color=orange>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
#else
            if (!editorOnly)
            {
                Debug.Log($"<color=orange>{TypeToString(type)} {LogTypeToString(logType)} {message}</color>");
            }
#endif
        }
        public static void e(object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.LogError($"{message}");
#else
            if (!editorOnly)
            {
                Debug.LogError($"{message}");
            }
#endif
        }
        public static void e(Type type, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.LogError($"{TypeToString(type)} {message}");
#else
            if (!editorOnly)
            {
                Debug.LogError($"{TypeToString(type)} {message}");
            }
#endif
        }        
        public static void e(Type type, LogType logType, object message, bool editorOnly = true)
        {
#if UNITY_EDITOR
            Debug.LogError($"{TypeToString(type)} {LogTypeToString(logType)} {message}");
#else
            if (!editorOnly)
            {
                Debug.LogError($"{TypeToString(type)} {LogTypeToString(logType)} {message}");
            }
#endif
        }
        private static string LogTypeToString(LogType type)
        {
            switch (type)
            {
                case LogType.INFO:
                    return "[INFO]";
                case LogType.WARN:
                    return "[WARN]";
                case LogType.ERROR:
                    return "[ERROR]";
                case LogType.FAIL:
                    return "[FAIL]";
                case LogType.SUCCESS:
                    return "[SUCCESS]";
                case LogType.TODO:
                    return "[TODO]";
                default:
                    return "[LOG]";
            }
        }
        private static string TypeToString(Type type)
        {
            return $"[{type.ToString().Split(".").Last()}]";
        }
    }

}
