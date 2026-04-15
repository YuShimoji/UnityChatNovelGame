using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectFoundPhone.EditorTools
{
    public static class MissingScriptScanner
    {
        private const string DebugChatScenePath = "Assets/Scenes/DebugChatScene.unity";

        [MenuItem("Tools/FoundPhone/Verification/Scan DebugChatScene Missing Scripts")]
        public static void ScanDebugChatSceneMissingScripts()
        {
            ScanSceneForMissingScripts(DebugChatScenePath, removeMissingScripts: false);
        }

        [MenuItem("Tools/FoundPhone/Verification/Scan DebugChatScene Prefab Dependencies")]
        public static void ScanDebugChatScenePrefabDependencies()
        {
            ScanPrefabDependencies(DebugChatScenePath, removeMissingScripts: false);
        }

        public static void ScanDebugChatSceneMissingScriptsBatch()
        {
            ScanSceneForMissingScripts(DebugChatScenePath, removeMissingScripts: false);
            EditorApplication.Exit(0);
        }

        public static void ScanDebugChatScenePrefabDependenciesBatch()
        {
            ScanPrefabDependencies(DebugChatScenePath, removeMissingScripts: false);
            EditorApplication.Exit(0);
        }

        public static void CleanupDebugChatScenePrefabDependenciesBatch()
        {
            ScanPrefabDependencies(DebugChatScenePath, removeMissingScripts: true);
            EditorApplication.Exit(0);
        }

        public static void CleanupDebugChatSceneMissingScriptsBatch()
        {
            bool removedAny = ScanSceneForMissingScripts(DebugChatScenePath, removeMissingScripts: true);
            EditorApplication.Exit(removedAny ? 0 : 0);
        }

        private static bool ScanSceneForMissingScripts(string scenePath, bool removeMissingScripts)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var findings = new List<string>();
            int removedCount = 0;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                ScanRecursive(root.transform, findings, ref removedCount, removeMissingScripts);
            }

            var log = new StringBuilder();
            log.AppendLine($"MissingScriptScanner: scene={scenePath}");
            log.AppendLine($"MissingScriptScanner: findings={findings.Count}");

            for (int i = 0; i < findings.Count; i++)
            {
                log.AppendLine(findings[i]);
            }

            if (removeMissingScripts && removedCount > 0)
            {
                log.AppendLine($"MissingScriptScanner: removed={removedCount}");
                EditorSceneManager.SaveScene(scene);
            }
            else
            {
                log.AppendLine("MissingScriptScanner: removed=0");
            }

            Debug.Log(log.ToString());
            return removedCount > 0;
        }

        private static void ScanPrefabDependencies(string scenePath, bool removeMissingScripts)
        {
            string[] dependencies = AssetDatabase.GetDependencies(scenePath, true);
            var findings = new List<string>();
            int removedCount = 0;

            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependency = dependencies[i];
                if (!dependency.EndsWith(".prefab"))
                {
                    continue;
                }

                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(dependency);
                try
                {
                    ScanRecursive(prefabRoot.transform, findings, ref removedCount, removeMissingScripts);
                    if (removeMissingScripts && removedCount > 0)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, dependency);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            var log = new StringBuilder();
            log.AppendLine($"MissingScriptScanner: dependency-scene={scenePath}");
            log.AppendLine($"MissingScriptScanner: dependency-findings={findings.Count}");
            for (int i = 0; i < findings.Count; i++)
            {
                log.AppendLine(findings[i]);
            }

            log.AppendLine($"MissingScriptScanner: dependency-removed={removedCount}");
            Debug.Log(log.ToString());
        }

        private static void ScanRecursive(Transform current, List<string> findings, ref int removedCount, bool removeMissingScripts)
        {
            GameObject go = current.gameObject;
            int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (missingCount > 0)
            {
                string prefabPath = string.Empty;
                Object prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (prefabSource != null)
                {
                    prefabPath = AssetDatabase.GetAssetPath(prefabSource);
                }

                findings.Add($"- path={GetHierarchyPath(current)} missing={missingCount} prefab={prefabPath}");

                if (removeMissingScripts)
                {
                    removedCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                }
            }

            for (int i = 0; i < current.childCount; i++)
            {
                ScanRecursive(current.GetChild(i), findings, ref removedCount, removeMissingScripts);
            }
        }

        private static string GetHierarchyPath(Transform current)
        {
            var parts = new Stack<string>();
            Transform cursor = current;
            while (cursor != null)
            {
                parts.Push(cursor.name);
                cursor = cursor.parent;
            }

            return string.Join("/", parts);
        }
    }
}
