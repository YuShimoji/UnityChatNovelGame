using UnityEngine;

namespace ProjectFoundPhone.Logging
{
    /// <summary>
    /// Custom debug logging wrapper for ProjectFoundPhone
    /// Usage: using ProjectFoundPhone.Debug; then call Debug.Log("message"), Debug.LogWarning("message"), Debug.LogError("message")
    /// </summary>
    public static class Debug
    {
        private const string PREFIX = "[ProjectFoundPhone]";

        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"{PREFIX} {message}");
        }

        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"{PREFIX} {message}");
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"{PREFIX} {message}");
        }
    }
}
