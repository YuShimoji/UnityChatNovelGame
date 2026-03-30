#if UNITY_EDITOR
using System;
using System.Linq;
using ProjectFoundPhone.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    public sealed class ContentPipelineWindow : EditorWindow
    {
        private const string ContentAuthoringScenePath = "Assets/Scenes/ContentAuthoring.unity";

        private string[] m_NodeNames = Array.Empty<string>();
        private int m_SelectedNodeIndex;

        [MenuItem("Tools/FoundPhone/Content Pipeline")]
        public static void ShowWindow()
        {
            var window = GetWindow<ContentPipelineWindow>("Content Pipeline");
            window.minSize = new Vector2(520, 260);
            window.RefreshNodes();
        }

        private void OnEnable()
        {
            RefreshNodes();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Content Pipeline", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Yarn の静的確認、SO 同期、ContentAuthoring シーンへの StartNode 適用を 1 つの導線にまとめます。",
                MessageType.Info);

            EditorGUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open Yarn Validator", GUILayout.Height(28)))
                {
                    YarnContentValidator.ShowWindow();
                }

                if (GUILayout.Button("Sync Authoring Assets", GUILayout.Height(28)))
                {
                    int changedCount = YarnSOGenerator.SyncAllAuthoringAssets();
                    RefreshNodes();
                    EditorUtility.DisplayDialog(
                        "Content Pipeline",
                        changedCount == 0
                            ? "同期対象はありませんでした。"
                            : $"同期完了: {changedCount} 件のアセットを生成/更新しました。",
                        "OK");
                }
            }

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open ContentAuthoring Scene"))
                {
                    OpenContentAuthoringScene();
                }

                if (GUILayout.Button("Refresh Nodes"))
                {
                    RefreshNodes();
                }

                if (GUILayout.Button("Use Recommended Node"))
                {
                    SelectRecommendedNode();
                }
            }

            EditorGUILayout.Space(6);
            DrawNodeSelector();

            EditorGUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(m_NodeNames.Length == 0))
                {
                    if (GUILayout.Button("Apply Node To Scene", GUILayout.Height(30)))
                    {
                        ApplySelectedNodeToScene(enterPlayMode: false);
                    }

                    if (GUILayout.Button("Play ContentAuthoring", GUILayout.Height(30)))
                    {
                        ApplySelectedNodeToScene(enterPlayMode: true);
                    }
                }
            }
        }

        private void DrawNodeSelector()
        {
            if (m_NodeNames.Length == 0)
            {
                EditorGUILayout.HelpBox("active/ 内の Yarn node を取得できませんでした。", MessageType.Warning);
                return;
            }

            m_SelectedNodeIndex = Mathf.Clamp(m_SelectedNodeIndex, 0, m_NodeNames.Length - 1);
            m_SelectedNodeIndex = EditorGUILayout.Popup("Start Node", m_SelectedNodeIndex, m_NodeNames);

            string selectedNode = m_NodeNames[m_SelectedNodeIndex];
            EditorGUILayout.HelpBox(
                $"選択中: {selectedNode}\nScene へ適用すると ScenarioManager の StartNode と AutoStartYarn を更新します。",
                MessageType.None);
        }

        private void RefreshNodes()
        {
            m_NodeNames = YarnSOGenerator.GetActiveYarnNodeNames();
            SelectRecommendedNode();
        }

        private void SelectRecommendedNode()
        {
            if (m_NodeNames.Length == 0)
            {
                m_SelectedNodeIndex = 0;
                return;
            }

            string recommendedNode = YarnSOGenerator.GetRecommendedStartNode(m_NodeNames);
            int index = Array.IndexOf(m_NodeNames, recommendedNode);
            m_SelectedNodeIndex = index >= 0 ? index : 0;
        }

        private static void OpenContentAuthoringScene()
        {
            EditorSceneManager.OpenScene(ContentAuthoringScenePath, OpenSceneMode.Single);
        }

        private void ApplySelectedNodeToScene(bool enterPlayMode)
        {
            if (m_NodeNames.Length == 0)
            {
                return;
            }

            OpenContentAuthoringScene();

            ScenarioManager scenarioManager = FindScenarioManagerInOpenScene();
            if (scenarioManager == null)
            {
                EditorUtility.DisplayDialog(
                    "Content Pipeline",
                    "ContentAuthoring シーン内に ScenarioManager が見つかりませんでした。",
                    "OK");
                return;
            }

            string selectedNode = m_NodeNames[m_SelectedNodeIndex];
            SerializedObject serializedObject = new SerializedObject(scenarioManager);
            serializedObject.FindProperty("m_StartNode").stringValue = selectedNode;
            serializedObject.FindProperty("m_AutoStartYarn").boolValue = true;
            serializedObject.FindProperty("m_DebugScenarioID").stringValue = string.Empty;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(scenarioManager);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            if (enterPlayMode && !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = true;
            }
        }

        private static ScenarioManager FindScenarioManagerInOpenScene()
        {
            return Resources.FindObjectsOfTypeAll<ScenarioManager>()
                .FirstOrDefault(manager => manager.gameObject.scene.path == ContentAuthoringScenePath);
        }
    }
}
#endif
