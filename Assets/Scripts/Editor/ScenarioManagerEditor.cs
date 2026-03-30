#if YARN_SPINNER
using System;
using System.Linq;
using ProjectFoundPhone.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Yarn.Unity;

namespace ProjectFoundPhone.Editor
{
    [CustomEditor(typeof(ScenarioManager))]
    public sealed class ScenarioManagerEditor : UnityEditor.Editor
    {
        private const string ContentAuthoringScenePath = "Assets/Scenes/ContentAuthoring.unity";

        private SerializedProperty m_DialogueRunnerProperty;
        private SerializedProperty m_ChatControllerProperty;
        private SerializedProperty m_StartNodeProperty;
        private SerializedProperty m_AutoStartYarnProperty;
        private SerializedProperty m_DebugScenarioIdProperty;

        private void OnEnable()
        {
            m_DialogueRunnerProperty = serializedObject.FindProperty("m_DialogueRunner");
            m_ChatControllerProperty = serializedObject.FindProperty("m_ChatController");
            m_StartNodeProperty = serializedObject.FindProperty("m_StartNode");
            m_AutoStartYarnProperty = serializedObject.FindProperty("m_AutoStartYarn");
            m_DebugScenarioIdProperty = serializedObject.FindProperty("m_DebugScenarioID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_DialogueRunnerProperty);
            EditorGUILayout.PropertyField(m_ChatControllerProperty);
            DrawStartNodeField();
            EditorGUILayout.PropertyField(m_AutoStartYarnProperty);
            EditorGUILayout.PropertyField(m_DebugScenarioIdProperty);

            EditorGUILayout.Space();
            DrawContentAuthoringActions();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStartNodeField()
        {
            DialogueRunner dialogueRunner = m_DialogueRunnerProperty.objectReferenceValue as DialogueRunner;
            YarnProject yarnProject = dialogueRunner != null ? dialogueRunner.YarnProject : null;
            string[] availableNodes = yarnProject?.NodeNames?
                .Where(nodeName => string.IsNullOrWhiteSpace(nodeName) == false)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(nodeName => nodeName, StringComparer.Ordinal)
                .ToArray()
                ?? Array.Empty<string>();

            if (availableNodes.Length == 0)
            {
                EditorGUILayout.PropertyField(m_StartNodeProperty);
                EditorGUILayout.HelpBox("YarnProject の node 一覧を取得できません。DialogueRunner の YarnProject を確認してください。", MessageType.Info);
                return;
            }

            string currentNode = string.IsNullOrWhiteSpace(m_StartNodeProperty.stringValue)
                ? availableNodes[0]
                : m_StartNodeProperty.stringValue;

            int currentIndex = Array.IndexOf(availableNodes, currentNode);
            bool nodeMissing = currentIndex < 0;
            if (nodeMissing)
            {
                currentIndex = 0;
            }

            int selectedIndex = EditorGUILayout.Popup("Start Node", currentIndex, availableNodes);
            if (selectedIndex >= 0 && selectedIndex < availableNodes.Length)
            {
                m_StartNodeProperty.stringValue = availableNodes[selectedIndex];
            }

            if (nodeMissing)
            {
                EditorGUILayout.HelpBox($"現在の Start Node '{currentNode}' は YarnProject に存在しません。利用可能な node に合わせて更新しました。", MessageType.Warning);
            }
        }

        private void DrawContentAuthoringActions()
        {
            EditorGUILayout.LabelField("Content Authoring", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Open Scene"))
                {
                    EditorSceneManager.OpenScene(ContentAuthoringScenePath, OpenSceneMode.Single);
                }

                if (GUILayout.Button("Rebuild Scene"))
                {
                    VerticalSliceSceneSetup.SetupContentAuthoringScene();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use Recommended Node"))
                {
                    m_StartNodeProperty.stringValue = YarnSOGenerator.GetRecommendedStartNode();
                }

                if (GUILayout.Button("Open Pipeline"))
                {
                    ContentPipelineWindow.ShowWindow();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Validate Runtime"))
                {
                    ContentAuthoringBatchValidator.ValidateFromMenu();
                }

                if (GUILayout.Button("Sync Authoring Assets"))
                {
                    YarnSOGenerator.SyncAllAuthoringAssetsFromMenu();
                }
            }

            EditorGUILayout.Space(4);

            bool isCorrectScene = IsContentAuthoringSceneActive();
            bool isPlaying = EditorApplication.isPlaying;

            using (new EditorGUI.DisabledGroupScope(!isCorrectScene || isPlaying))
            {
                if (GUILayout.Button("Play from Node", GUILayout.Height(30)))
                {
                    PlayFromSelectedNode();
                }
            }

            if (!isCorrectScene)
            {
                EditorGUILayout.HelpBox("ContentAuthoring シーンを開いてください。", MessageType.Info);
            }
            else if (isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode 中は使用できません。", MessageType.Info);
            }
        }

        private bool IsContentAuthoringSceneActive()
        {
            return EditorSceneManager.GetActiveScene().path == ContentAuthoringScenePath;
        }

        private void PlayFromSelectedNode()
        {
            m_AutoStartYarnProperty.boolValue = true;
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorApplication.isPlaying = true;
        }
    }
}
#endif
