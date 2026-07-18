#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public static class WriterCockpitNavigation
    {
        public static YarnSOGenerator.YarnNodeSourceLocation[] FilterNodeLocations(
            IEnumerable<YarnSOGenerator.YarnNodeSourceLocation> locations,
            string filter)
        {
            IEnumerable<YarnSOGenerator.YarnNodeSourceLocation> source =
                locations ?? Enumerable.Empty<YarnSOGenerator.YarnNodeSourceLocation>();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                string search = filter.Trim();
                source = source.Where(location =>
                    location.NodeName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return source
                .OrderBy(location => location.NodeName, StringComparer.Ordinal)
                .ThenBy(location => location.AssetPath, StringComparer.Ordinal)
                .ThenBy(location => location.TitleLine)
                .ToArray();
        }

        public static bool TryOpenAssetAtLine(string assetPath, int line, out string status)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                status = "Source file is not available.";
                return false;
            }

            string normalizedAssetPath = assetPath.Replace('\\', '/');
            string fullPath = Path.IsPathRooted(normalizedAssetPath)
                ? normalizedAssetPath
                : Path.GetFullPath(normalizedAssetPath);

            if (!File.Exists(fullPath))
            {
                status = $"Source file is missing: {normalizedAssetPath}";
                return false;
            }

            if (Path.IsPathRooted(normalizedAssetPath))
            {
                normalizedAssetPath = FileUtil.GetProjectRelativePath(fullPath);
            }

            IExternalCodeEditor editor = CodeEditor.CurrentEditor;
            bool opensByFileAssociation = string.Equals(
                editor?.GetType().FullName,
                "UnityEditor.DefaultExternalCodeEditor",
                StringComparison.Ordinal);
            string editorPath = CodeEditor.CurrentEditorPath;
            if (string.IsNullOrWhiteSpace(editorPath) ||
                opensByFileAssociation ||
                (!File.Exists(editorPath) && !Directory.Exists(editorPath)))
            {
                status =
                    "External Script Editor is not configured. Set it in Unity Preferences > External Tools.";
                return false;
            }

            int targetLine = Mathf.Max(1, line);
            if (editor == null || !editor.OpenProject(fullPath, targetLine, 1))
            {
                status = $"The configured external editor could not open {normalizedAssetPath}:{targetLine}.";
                return false;
            }

            status = $"Opened {normalizedAssetPath}:{targetLine}.";
            return true;
        }
    }
}
#endif
