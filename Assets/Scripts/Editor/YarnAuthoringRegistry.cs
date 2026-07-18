#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ProjectFoundPhone.Data;
using UnityEditor;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// Editor diagnostics use the runtime registration source and CharacterProfile assets
    /// instead of maintaining parallel command and character lists.
    /// </summary>
    public static class YarnAuthoringRegistry
    {
        private const string RuntimeScriptsRoot = "Assets/Scripts";
        private const string CharacterAssetsRoot = "Assets/Resources/Characters";

        private static readonly Regex CommandHandlerRegex = new Regex(
            @"(?m)^(?![ \t]*//)[^\r\n]*?\.AddCommandHandler(?:[ \t]*<[^>\r\n]*>)?[ \t]*\([ \t]*""(?<name>[A-Za-z_]\w*)""",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly string[] NonProfileSpeakerIds =
        {
            "system",
            "narrator"
        };

        public static HashSet<string> GetRegisteredCommandNames()
        {
            string fullRoot = Path.GetFullPath(RuntimeScriptsRoot);
            var commands = new HashSet<string>(StringComparer.Ordinal);

            foreach (string sourcePath in Directory.GetFiles(fullRoot, "*.cs", SearchOption.AllDirectories))
            {
                string normalizedPath = sourcePath.Replace('\\', '/');
                if (normalizedPath.Contains("/Editor/") || normalizedPath.Contains("/Tests/"))
                {
                    continue;
                }

                commands.UnionWith(ExtractRegisteredCommandNames(File.ReadAllText(sourcePath)));
            }

            return commands;
        }

        public static string[] ExtractRegisteredCommandNames(string sourceText)
        {
            if (string.IsNullOrWhiteSpace(sourceText))
            {
                return Array.Empty<string>();
            }

            return CommandHandlerRegex.Matches(sourceText)
                .Cast<Match>()
                .Select(match => match.Groups["name"].Value)
                .Where(command => !string.IsNullOrWhiteSpace(command))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(command => command, StringComparer.Ordinal)
                .ToArray();
        }

        public static HashSet<string> GetKnownCharacterIds()
        {
            var characterIds = new HashSet<string>(NonProfileSpeakerIds, StringComparer.OrdinalIgnoreCase);
            string[] guids = AssetDatabase.FindAssets("t:CharacterProfile", new[] { CharacterAssetsRoot });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                CharacterProfile profile = AssetDatabase.LoadAssetAtPath<CharacterProfile>(assetPath);
                if (profile != null && !string.IsNullOrWhiteSpace(profile.CharacterID))
                {
                    characterIds.Add(profile.CharacterID);
                }
            }

            return characterIds;
        }
    }
}
#endif
