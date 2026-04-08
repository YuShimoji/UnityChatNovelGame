using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ProjectFoundPhone.AgentSessionLog
{
    /// <summary>
    /// デバッグセッション用 NDJSON ログ（ワークスペース直下 debug-3313e1.log）。
    /// </summary>
    public static class AgentDebugSessionLog
    {
        private const string c_LogFileName = "debug-3313e1.log";
        private const string c_SessionId = "3313e1";

        public static void Write(string hypothesisId, string location, string message, string dataJsonObject)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            try
            {
                string path = Path.Combine(Application.dataPath, "..", c_LogFileName);
                long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var sb = new StringBuilder(256);
                sb.Append("{\"sessionId\":\"").Append(c_SessionId).Append("\",");
                sb.Append("\"hypothesisId\":\"").Append(EscapeJson(hypothesisId)).Append("\",");
                sb.Append("\"location\":\"").Append(EscapeJson(location)).Append("\",");
                sb.Append("\"message\":\"").Append(EscapeJson(message)).Append("\",");
                sb.Append("\"data\":").Append(string.IsNullOrEmpty(dataJsonObject) ? "{}" : dataJsonObject).Append(",");
                sb.Append("\"timestamp\":").Append(ts).Append("}\n");
                File.AppendAllText(path, sb.ToString());
            }
            catch
            {
                // ログ失敗でゲームを止めない
            }
#endif
        }

        /// <summary>JSON 文字列値用のエスケープ（呼び出し側が data を組み立てる場合に利用可）。</summary>
        public static string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
