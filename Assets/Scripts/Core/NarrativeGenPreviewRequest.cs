using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ProjectFoundPhone.Editor")]

#if UNITY_EDITOR
namespace ProjectFoundPhone.Core
{
    internal static class NarrativeGenPreviewRequest
    {
        public const string ScenePath = "Assets/Scenes/DebugChatScene.unity";
        private const string ModelKey = "FoundPhone.NarrativeGenPreview.ModelPath";

        public static void Queue(string file) => UnityEditor.SessionState.SetString(ModelKey, file);

        public static string Consume(string scenePath)
        {
            if (scenePath != ScenePath)
            {
                return null;
            }

            string file = UnityEditor.SessionState.GetString(ModelKey, string.Empty);
            Clear();
            return string.IsNullOrEmpty(file) ? null : file;
        }

        public static void Clear() => UnityEditor.SessionState.EraseString(ModelKey);
    }
}
#endif
