namespace NooSphere
{
    using UnityEngine;

    public static class Debug
    {
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object message, Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogWarning(object message, Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void LogError(object message, Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }
    }
}