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
        private Vector2 m_NodeListScrollPosition;
        private Vector2 m_DiagnosticScrollPosition;
        private YarnSOGenerator.AuthoringScanSummary m_ScanSummary;
        private YarnContentValidator.ValidationSummary m_LastValidationSummary;
        private YarnContentValidator.ValidationReport m_LastValidationReport;
        private string[] m_NodeNames = Array.Empty<string>();
        private YarnSOGenerator.YarnNodeSourceLocation[] m_NodeLocations =
            Array.Empty<YarnSOGenerator.YarnNodeSourceLocation>();
        private string m_RecommendedNode = "Start";
        private string m_NodeFilter = string.Empty;
        private string m_LastAction = "未実行";
        private string m_SourceOpenStatus;
        private string m_ContentAuthoringStatus = "未確認";
        private string m_SaveStatus = "未確認";
        private string m_ScanError;
        private int m_SelectedNodeIndex;
        private int m_LastSyncChangedCount;
        private bool m_HasScanSummary;
        private bool m_HasValidationSummary;
        private bool m_HasSyncResult;
        private bool m_ShowErrors = true;
        private bool m_ShowWarnings = true;
        private bool m_ShowInfo = true;

        [MenuItem("Tools/FoundPhone/Writer Cockpit", false, 19)]
        public static void ShowWindow()
        {
            var window = GetWindow<WriterCockpitWindow>("Writer Cockpit");
            window.minSize = new Vector2(680, 520);
            window.RefreshAuthoringStatus(recordAsAction: false);
        }

        private void OnEnable()
        {
            // ValidationReport is intentionally session-only. Do not retain a serialized
            // summary without its drilldown rows across a domain reload.
            m_LastValidationReport = null;
            m_HasValidationSummary = false;
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
            DrawDiagnostics();

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
            EditorGUILayout.LabelField("Node Search And Source", EditorStyles.boldLabel);

            if (m_NodeLocations.Length == 0)
            {
                EditorGUILayout.HelpBox("active/ 内の Yarn node を取得できませんでした。", MessageType.Warning);
                return;
            }

            m_NodeFilter = EditorGUILayout.TextField("Node Search", m_NodeFilter);
            YarnSOGenerator.YarnNodeSourceLocation[] filteredLocations =
                WriterCockpitNavigation.FilterNodeLocations(m_NodeLocations, m_NodeFilter);

            EditorGUILayout.LabelField(
                $"{filteredLocations.Length} / {m_NodeLocations.Length} nodes",
                EditorStyles.miniLabel);

            float listHeight = Mathf.Clamp(filteredLocations.Length * 23f + 4f, 48f, 170f);
            m_NodeListScrollPosition = EditorGUILayout.BeginScrollView(
                m_NodeListScrollPosition,
                GUILayout.Height(listHeight));

            if (filteredLocations.Length == 0)
            {
                EditorGUILayout.HelpBox("一致するNodeがありません。検索語を変更してください。", MessageType.Warning);
            }

            string selectedNode = GetSelectedNodeOrFallback();
            foreach (YarnSOGenerator.YarnNodeSourceLocation location in filteredLocations)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    bool selected = string.Equals(selectedNode, location.NodeName, StringComparison.Ordinal);
                    bool choose = GUILayout.Toggle(
                        selected,
                        location.NodeName,
                        "Button",
                        GUILayout.Height(21));
                    if (choose && !selected)
                    {
                        int fullIndex = Array.IndexOf(m_NodeNames, location.NodeName);
                        if (fullIndex >= 0)
                        {
                            m_SelectedNodeIndex = fullIndex;
                            selectedNode = location.NodeName;
                            m_SourceOpenStatus = null;
                        }
                    }

                    EditorGUILayout.LabelField(
                        $"{Path.GetFileName(location.AssetPath)}:{location.TitleLine}",
                        EditorStyles.miniLabel,
                        GUILayout.Width(210));
                }
            }

            EditorGUILayout.EndScrollView();

            if (!TryGetSelectedNodeLocation(out YarnSOGenerator.YarnNodeSourceLocation selectedLocation))
            {
                EditorGUILayout.HelpBox("選択Nodeのsource locationが見つかりません。Refreshしてください。", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Selected Node", selectedLocation.NodeName);
            EditorGUILayout.LabelField(
                "Source",
                $"{selectedLocation.AssetPath}:{selectedLocation.TitleLine}",
                EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Open Selected Source At Line", GUILayout.Height(26)))
            {
                OpenSourceAtLine(selectedLocation.AssetPath, selectedLocation.TitleLine);
            }

            if (!string.IsNullOrWhiteSpace(m_SourceOpenStatus))
            {
                EditorGUILayout.HelpBox(m_SourceOpenStatus, MessageType.None);
            }
        }

        private void DrawDiagnostics()
        {
            EditorGUILayout.LabelField("Validator Diagnostics", EditorStyles.boldLabel);
            if (m_LastValidationReport == null)
            {
                EditorGUILayout.HelpBox(
                    "Validate All Yarn Files または Validate Then Sync でfile/line付き結果を表示します。",
                    MessageType.Info);
                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                m_ShowErrors = GUILayout.Toggle(m_ShowErrors, $"Error {m_LastValidationSummary.ErrorCount}", "Button");
                m_ShowWarnings = GUILayout.Toggle(m_ShowWarnings, $"Warning {m_LastValidationSummary.WarningCount}", "Button");
                m_ShowInfo = GUILayout.Toggle(m_ShowInfo, $"Info {m_LastValidationSummary.InfoCount}", "Button");
            }

            YarnContentValidator.ValidationResult[] visibleResults = m_LastValidationReport.Results
                .Where(result =>
                    result.Level == YarnContentValidator.ValidationLevel.Error && m_ShowErrors ||
                    result.Level == YarnContentValidator.ValidationLevel.Warning && m_ShowWarnings ||
                    result.Level == YarnContentValidator.ValidationLevel.Info && m_ShowInfo)
                .ToArray();

            m_DiagnosticScrollPosition = EditorGUILayout.BeginScrollView(
                m_DiagnosticScrollPosition,
                GUILayout.Height(Mathf.Clamp(visibleResults.Length * 42f + 4f, 58f, 220f)));

            foreach (YarnContentValidator.ValidationResult result in visibleResults)
            {
                DrawDiagnosticResult(result);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawDiagnosticResult(YarnContentValidator.ValidationResult result)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(result.Level.ToString(), GUILayout.Width(58));
                using (new EditorGUILayout.VerticalScope())
                {
                    string location = string.IsNullOrWhiteSpace(result.File) || result.Line <= 0
                        ? "(summary)"
                        : $"{result.File}:{result.Line}";
                    EditorGUILayout.LabelField(location, EditorStyles.miniLabel);
                    EditorGUILayout.LabelField(result.Message, EditorStyles.wordWrappedLabel);
                }

                using (new EditorGUI.DisabledGroupScope(
                    string.IsNullOrWhiteSpace(result.File) || result.Line <= 0))
                {
                    if (GUILayout.Button("Open", GUILayout.Width(54), GUILayout.Height(34)))
                    {
                        OpenSourceAtLine(result.File, result.Line);
                    }
                }
            }
        }

        private void RefreshAuthoringStatus(bool recordAsAction)
        {
            string previousSelectedNode = GetSelectedNodeOrFallback();

            try
            {
                m_ScanSummary = YarnSOGenerator.GetAuthoringScanSummary();
                m_NodeNames = m_ScanSummary.NodeNames ?? Array.Empty<string>();
                m_NodeLocations = m_ScanSummary.NodeLocations ??
                    Array.Empty<YarnSOGenerator.YarnNodeSourceLocation>();
                m_RecommendedNode = YarnSOGenerator.GetRecommendedStartNode(m_NodeNames);
                m_HasScanSummary = true;
                m_ScanError = null;
                RestoreSelection(previousSelectedNode);
            }
            catch (Exception ex)
            {
                m_NodeNames = Array.Empty<string>();
                m_NodeLocations = Array.Empty<YarnSOGenerator.YarnNodeSourceLocation>();
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
                m_LastValidationReport = YarnContentValidator.GetValidationReport();
                m_LastValidationSummary = m_LastValidationReport.Summary;
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
                m_LastValidationReport = YarnContentValidator.GetValidationReport();
                m_LastValidationSummary = m_LastValidationReport.Summary;
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

        private bool TryGetSelectedNodeLocation(
            out YarnSOGenerator.YarnNodeSourceLocation selectedLocation)
        {
            selectedLocation = default;
            if (!TryGetSelectedNode(out string selectedNode))
            {
                return false;
            }

            foreach (YarnSOGenerator.YarnNodeSourceLocation location in m_NodeLocations)
            {
                if (string.Equals(location.NodeName, selectedNode, StringComparison.Ordinal))
                {
                    selectedLocation = location;
                    return true;
                }
            }

            return false;
        }

        private void OpenSourceAtLine(string assetPath, int line)
        {
            WriterCockpitNavigation.TryOpenAssetAtLine(assetPath, line, out m_SourceOpenStatus);
            m_LastAction = m_SourceOpenStatus;
            Repaint();
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
