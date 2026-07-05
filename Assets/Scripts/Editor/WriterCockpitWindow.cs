#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public sealed class WriterCockpitWindow : EditorWindow
    {
        private const string ActiveYarnDir = "Assets/Resources/Yarn/active";
        private const int ManualSaveSlotCount = 3;

        private Vector2 m_ScrollPosition;
        private YarnSOGenerator.AuthoringScanSummary m_ScanSummary;
        private YarnContentValidator.ValidationSummary m_LastValidationSummary;
        private string[] m_NodeNames = Array.Empty<string>();
        private string m_RecommendedNode = "Start";
        private string m_LastAction = "未実行";
        private string m_ContentAuthoringStatus = "未確認";
        private string m_SaveStatus = "未確認";
        private string m_ScanError;
        private int m_SelectedNodeIndex;
        private int m_LastSyncChangedCount;
        private bool m_HasScanSummary;
        private bool m_HasValidationSummary;
        private bool m_HasSyncResult;

        [MenuItem("Tools/FoundPhone/Writer Cockpit", false, 19)]
        public static void ShowWindow()
        {
            var window = GetWindow<WriterCockpitWindow>("Writer Cockpit");
            window.minSize = new Vector2(680, 520);
            window.RefreshAuthoringStatus(recordAsAction: false);
        }

        private void OnEnable()
        {
            RefreshAuthoringStatus(recordAsAction: false);
        }

        private void OnGUI()
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Writer / Designer Cockpit", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "外部エディタで Yarn を保存 → Unity に戻る → Refresh / Validate → Sync → Apply または Play。",
                MessageType.Info);

            DrawStatusSection();

            EditorGUILayout.Space(8);
            DrawActionSection();

            EditorGUILayout.Space(8);
            DrawNodeSelector();

            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox($"Last Action: {m_LastAction}", MessageType.None);

            EditorGUILayout.EndScrollView();
        }

        private void DrawStatusSection()
        {
            EditorGUILayout.LabelField("Authoring Status", EditorStyles.boldLabel);

            if (!string.IsNullOrEmpty(m_ScanError))
            {
                EditorGUILayout.HelpBox(m_ScanError, MessageType.Error);
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawStatusRow("Active Yarn files", m_HasScanSummary ? m_ScanSummary.YarnFileCount.ToString() : "-");
                DrawStatusRow("Active Yarn nodes", m_HasScanSummary ? m_ScanSummary.NodeCount.ToString() : "-");
                DrawStatusRow("Recommended start node", m_RecommendedNode);
                DrawStatusRow("Selected start node", GetSelectedNodeForDisplay());
                DrawStatusRow("Pending authoring sync", BuildPendingSyncText());
                DrawStatusRow("Last validation", m_HasValidationSummary ? m_LastValidationSummary.ToDisplayText() : "未実行");
                DrawStatusRow("Last sync", BuildLastSyncText());
                DrawStatusRow("ContentAuthoring", m_ContentAuthoringStatus);
                DrawStatusRow("Save / autosave", m_SaveStatus);
            }
        }

        private void DrawActionSection()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh Nodes", GUILayout.Height(28)))
                {
                    RefreshAuthoringStatus(recordAsAction: true);
                }

                if (GUILayout.Button("Validate All Yarn Files", GUILayout.Height(28)))
                {
                    RunValidation();
                }

                if (GUILayout.Button("Sync Authoring Assets", GUILayout.Height(28)))
                {
                    RunSync();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Validate Then Sync", GUILayout.Height(28)))
                {
                    RunValidateThenSync();
                }

                using (new EditorGUI.DisabledGroupScope(m_NodeNames.Length == 0))
                {
                    if (GUILayout.Button("Use Recommended Node", GUILayout.Height(28)))
                    {
                        SelectRecommendedNode();
                        m_LastAction = $"Recommended node selected: {GetSelectedNodeOrFallback()}";
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(m_NodeNames.Length == 0))
                {
                    if (GUILayout.Button("Apply Node To ContentAuthoring Scene", GUILayout.Height(30)))
                    {
                        ApplySelectedNodeToContentAuthoring(enterPlayMode: false);
                    }

                    if (GUILayout.Button("Play ContentAuthoring From Selected Node", GUILayout.Height(30)))
                    {
                        ApplySelectedNodeToContentAuthoring(enterPlayMode: true);
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Ping Active Yarn Directory", GUILayout.Height(26)))
                {
                    PingActiveYarnDirectory();
                }

                if (GUILayout.Button("Open Active Yarn Folder", GUILayout.Height(26)))
                {
                    OpenActiveYarnFolder();
                }
            }
        }

        private void DrawNodeSelector()
        {
            EditorGUILayout.LabelField("Start Node", EditorStyles.boldLabel);

            if (m_NodeNames.Length == 0)
            {
                EditorGUILayout.HelpBox("active/ 内の Yarn node を取得できませんでした。", MessageType.Warning);
                return;
            }

            m_SelectedNodeIndex = Mathf.Clamp(m_SelectedNodeIndex, 0, m_NodeNames.Length - 1);
            m_SelectedNodeIndex = EditorGUILayout.Popup("Selected Node", m_SelectedNodeIndex, m_NodeNames);
        }

        private void RefreshAuthoringStatus(bool recordAsAction)
        {
            string previousSelectedNode = GetSelectedNodeOrFallback();

            try
            {
                m_ScanSummary = YarnSOGenerator.GetAuthoringScanSummary();
                m_NodeNames = m_ScanSummary.NodeNames ?? Array.Empty<string>();
                m_RecommendedNode = YarnSOGenerator.GetRecommendedStartNode(m_NodeNames);
                m_HasScanSummary = true;
                m_ScanError = null;
                RestoreSelection(previousSelectedNode);
            }
            catch (Exception ex)
            {
                m_NodeNames = Array.Empty<string>();
                m_RecommendedNode = "Start";
                m_HasScanSummary = false;
                m_ScanError = $"Yarn authoring scan failed: {ex.Message}";
                m_SelectedNodeIndex = 0;
            }

            m_ContentAuthoringStatus = ContentPipelineWindow.GetContentAuthoringSceneStatus();
            m_SaveStatus = BuildReadOnlySaveStatus();

            if (recordAsAction)
            {
                m_LastAction = "Nodes and authoring status refreshed.";
            }

            Repaint();
        }

        private void RunValidation()
        {
            try
            {
                m_LastValidationSummary = YarnContentValidator.GetValidationSummary();
                m_HasValidationSummary = true;
                m_LastAction = $"Validation complete: {m_LastValidationSummary.ToDisplayText()}.";
            }
            catch (Exception ex)
            {
                m_LastAction = $"Validation failed: {ex.Message}";
            }
        }

        private void RunSync()
        {
            try
            {
                m_LastSyncChangedCount = YarnSOGenerator.SyncAllAuthoringAssets();
                m_HasSyncResult = true;
                RefreshAuthoringStatus(recordAsAction: false);
                m_LastAction = BuildSyncActionText("Sync complete", m_LastSyncChangedCount);
            }
            catch (Exception ex)
            {
                m_LastAction = $"Sync failed: {ex.Message}";
            }
        }

        private void RunValidateThenSync()
        {
            try
            {
                m_LastValidationSummary = YarnContentValidator.GetValidationSummary();
                m_HasValidationSummary = true;

                if (m_LastValidationSummary.HasErrors)
                {
                    m_LastAction = $"Validate stopped sync: {m_LastValidationSummary.ToDisplayText()}.";
                    return;
                }

                m_LastSyncChangedCount = YarnSOGenerator.SyncAllAuthoringAssets();
                m_HasSyncResult = true;
                RefreshAuthoringStatus(recordAsAction: false);
                m_LastAction = BuildSyncActionText("Validate passed, sync complete", m_LastSyncChangedCount);
            }
            catch (Exception ex)
            {
                m_LastAction = $"Validate then sync failed: {ex.Message}";
            }
        }

        private void ApplySelectedNodeToContentAuthoring(bool enterPlayMode)
        {
            if (!TryGetSelectedNode(out string selectedNode))
            {
                m_LastAction = "Apply skipped: Start Node が選択されていません。";
                return;
            }

            bool applied = ContentPipelineWindow.ApplyNodeToContentAuthoringScene(
                selectedNode,
                enterPlayMode,
                out string statusMessage);

            m_ContentAuthoringStatus = ContentPipelineWindow.GetContentAuthoringSceneStatus();
            m_LastAction = statusMessage;

            if (!applied)
            {
                EditorUtility.DisplayDialog("Writer Cockpit", statusMessage, "OK");
            }
        }

        private void SelectRecommendedNode()
        {
            if (m_NodeNames.Length == 0)
            {
                m_SelectedNodeIndex = 0;
                return;
            }

            int recommendedIndex = Array.IndexOf(m_NodeNames, m_RecommendedNode);
            m_SelectedNodeIndex = recommendedIndex >= 0 ? recommendedIndex : 0;
        }

        private void RestoreSelection(string previousSelectedNode)
        {
            if (m_NodeNames.Length == 0)
            {
                m_SelectedNodeIndex = 0;
                return;
            }

            int previousIndex = Array.IndexOf(m_NodeNames, previousSelectedNode);
            if (previousIndex >= 0)
            {
                m_SelectedNodeIndex = previousIndex;
                return;
            }

            SelectRecommendedNode();
        }

        private bool TryGetSelectedNode(out string selectedNode)
        {
            if (m_NodeNames.Length == 0)
            {
                selectedNode = string.Empty;
                return false;
            }

            m_SelectedNodeIndex = Mathf.Clamp(m_SelectedNodeIndex, 0, m_NodeNames.Length - 1);
            selectedNode = m_NodeNames[m_SelectedNodeIndex];
            return !string.IsNullOrWhiteSpace(selectedNode);
        }

        private string GetSelectedNodeOrFallback()
        {
            return TryGetSelectedNode(out string selectedNode) ? selectedNode : string.Empty;
        }

        private string GetSelectedNodeForDisplay()
        {
            if (m_NodeNames.Length == 0)
            {
                return "-";
            }

            int selectedIndex = Mathf.Clamp(m_SelectedNodeIndex, 0, m_NodeNames.Length - 1);
            return m_NodeNames[selectedIndex];
        }

        private string BuildPendingSyncText()
        {
            if (!m_HasScanSummary)
            {
                return "-";
            }

            return $"{m_ScanSummary.PendingChangeCount} pending (topics={m_ScanSummary.MissingTopicCount}, characters={m_ScanSummary.MissingCharacterCount}, channels={m_ScanSummary.ChannelSyncItemCount})";
        }

        private string BuildLastSyncText()
        {
            if (!m_HasSyncResult)
            {
                return "未実行";
            }

            return m_LastSyncChangedCount == 0
                ? "no changes"
                : $"{m_LastSyncChangedCount} asset(s) changed";
        }

        private static string BuildSyncActionText(string prefix, int changedCount)
        {
            return changedCount == 0
                ? $"{prefix}: no authoring asset changes."
                : $"{prefix}: {changedCount} authoring asset(s) changed.";
        }

        private static string BuildReadOnlySaveStatus()
        {
            string persistentPath = Application.persistentDataPath;
            int[] existingManualSlots = Enumerable.Range(0, ManualSaveSlotCount)
                .Where(slot => File.Exists(Path.Combine(persistentPath, $"SaveData_{slot}.json")))
                .ToArray();
            bool hasAutoSave = File.Exists(Path.Combine(persistentPath, "SaveData_99.json"));
            string manualSlots = existingManualSlots.Length == 0
                ? "none"
                : string.Join(", ", existingManualSlots.Select(slot => slot.ToString()));

            return $"read-only files: manual={existingManualSlots.Length}/{ManualSaveSlotCount} ({manualSlots}), autosave={(hasAutoSave ? "present" : "none")}";
        }

        private static void DrawStatusRow(string label, string value)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(170));
                EditorGUILayout.LabelField(value, EditorStyles.wordWrappedLabel);
            }
        }

        private void PingActiveYarnDirectory()
        {
            UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ActiveYarnDir);
            if (folder == null)
            {
                m_LastAction = $"Active Yarn directory not found in Project view: {ActiveYarnDir}";
                return;
            }

            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
            m_LastAction = $"Pinged active Yarn directory: {ActiveYarnDir}";
        }

        private void OpenActiveYarnFolder()
        {
            string fullPath = Path.GetFullPath(ActiveYarnDir);
            if (!Directory.Exists(fullPath))
            {
                m_LastAction = $"Active Yarn folder not found: {fullPath}";
                return;
            }

            EditorUtility.RevealInFinder(fullPath);
            m_LastAction = $"Opened active Yarn folder: {fullPath}";
        }
    }
}
#endif
