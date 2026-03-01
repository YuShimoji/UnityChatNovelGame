#if YARN_SPINNER
using System.Collections.Generic;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

namespace ProjectFoundPhone.Editor
{
    public static class VerticalSliceSceneSetup
    {
        private const string DebugChatScenePath = "Assets/Scenes/DebugChatScene.unity";
        private const string ContentAuthoringScenePath = "Assets/Scenes/ContentAuthoring.unity";
        private const string DefaultStartNode = "VerticalSlice_Start";
        private const string YarnProjectPath = "Assets/Resources/Yarn/Project.yarnproject";

        [MenuItem("Tools/Setup Vertical Slice Scene")]
        public static void SetupScene()
        {
            SetupSceneInternal(DebugChatScenePath, DefaultStartNode, "VerticalSliceSetup");
        }

        [MenuItem("Tools/Setup Content Authoring Scene")]
        public static void SetupContentAuthoringScene()
        {
            SetupSceneInternal(ContentAuthoringScenePath, DefaultStartNode, "ContentAuthoringSetup");
        }

        public static void SetupContentAuthoringSceneBatch()
        {
            SetupContentAuthoringScene();
        }

        private static void SetupSceneInternal(string scenePath, string startNode, string logPrefix)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.12f, 0.15f);
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();

            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject backgroundObj = CreateUIElement("Background", canvasObj.transform);
            Image backgroundImage = backgroundObj.AddComponent<Image>();
            backgroundImage.color = new Color(0.1f, 0.1f, 0.13f);
            StretchToFill(backgroundObj);

            GameObject scrollViewObj = CreateUIElement("ChatScrollView", canvasObj.transform);
            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            RectTransform scrollRectTransform = scrollViewObj.GetComponent<RectTransform>();
            scrollRectTransform.anchorMin = Vector2.zero;
            scrollRectTransform.anchorMax = Vector2.one;
            scrollRectTransform.offsetMin = new Vector2(0f, 150f);
            scrollRectTransform.offsetMax = Vector2.zero;

            GameObject viewportObj = CreateUIElement("Viewport", scrollViewObj.transform);
            StretchToFill(viewportObj);
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;
            Image viewportImage = viewportObj.AddComponent<Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 0.01f);

            GameObject contentObj = CreateUIElement("Content", viewportObj.transform);
            RectTransform contentTransform = contentObj.GetComponent<RectTransform>();
            contentTransform.anchorMin = new Vector2(0f, 1f);
            contentTransform.anchorMax = new Vector2(1f, 1f);
            contentTransform.pivot = new Vector2(0.5f, 1f);
            contentTransform.sizeDelta = Vector2.zero;

            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
            layoutGroup.spacing = 16;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            ContentSizeFitter contentSizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentTransform;
            scrollRect.viewport = viewportObj.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;

            GameObject typingInstance = null;
            GameObject typingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/TypingIndicator.prefab");
            if (typingPrefab != null)
            {
                typingInstance = PrefabUtility.InstantiatePrefab(typingPrefab, contentObj.transform) as GameObject;
                typingInstance.SetActive(false);
            }

            GameObject choiceContainerObj = CreateUIElement("ChoiceContainer", canvasObj.transform);
            RectTransform choiceTransform = choiceContainerObj.GetComponent<RectTransform>();
            choiceTransform.anchorMin = new Vector2(0.05f, 0.3f);
            choiceTransform.anchorMax = new Vector2(0.95f, 0.7f);
            choiceTransform.offsetMin = Vector2.zero;
            choiceTransform.offsetMax = Vector2.zero;

            VerticalLayoutGroup choiceLayoutGroup = choiceContainerObj.AddComponent<VerticalLayoutGroup>();
            choiceLayoutGroup.spacing = 12;
            choiceLayoutGroup.childControlHeight = true;
            choiceLayoutGroup.childControlWidth = true;
            choiceLayoutGroup.childForceExpandHeight = false;
            choiceLayoutGroup.childForceExpandWidth = true;
            choiceLayoutGroup.padding = new RectOffset(20, 20, 20, 20);
            choiceContainerObj.SetActive(false);

            GameObject choiceButtonTemplate = CreateChoiceButtonPrefab(logPrefix);

            GameObject footerObj = CreateUIElement("Footer", canvasObj.transform);
            RectTransform footerTransform = footerObj.GetComponent<RectTransform>();
            footerTransform.anchorMin = Vector2.zero;
            footerTransform.anchorMax = new Vector2(1f, 0f);
            footerTransform.offsetMin = Vector2.zero;
            footerTransform.offsetMax = new Vector2(0f, 150f);
            Image footerBackground = footerObj.AddComponent<Image>();
            footerBackground.color = new Color(0.15f, 0.15f, 0.18f);

            TMP_InputField inputField = CreateInputField(footerObj.transform);
            Button sendButton = CreateSendButton(footerObj.transform);

            ChatController chatController = scrollViewObj.AddComponent<ChatController>();
            SerializedObject chatControllerObject = new SerializedObject(chatController);
            chatControllerObject.Update();
            chatControllerObject.FindProperty("m_ScrollRect").objectReferenceValue = scrollRect;
            chatControllerObject.FindProperty("m_LayoutGroup").objectReferenceValue = layoutGroup;
            chatControllerObject.FindProperty("m_InputField").objectReferenceValue = inputField;
            chatControllerObject.FindProperty("m_SendButton").objectReferenceValue = sendButton;
            chatControllerObject.FindProperty("m_ChoiceContainer").objectReferenceValue = choiceContainerObj.transform;
            chatControllerObject.FindProperty("m_ChoiceButtonPrefab").objectReferenceValue = choiceButtonTemplate;

            GameObject bubblePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/MessageBubble.prefab");
            if (bubblePrefab != null)
            {
                chatControllerObject.FindProperty("m_MessageBubblePrefab").objectReferenceValue = bubblePrefab;
            }
            else
            {
                Debug.LogError($"{logPrefix}: MessageBubble.prefab not found.");
            }

            if (typingInstance != null)
            {
                chatControllerObject.FindProperty("m_TypingIndicator").objectReferenceValue = typingInstance;
            }

            chatControllerObject.ApplyModifiedProperties();

            GameObject dialogueSystemObj = new GameObject("DialogueSystem");
            dialogueSystemObj.AddComponent<InMemoryVariableStorage>();

            DialogueRunner dialogueRunner = dialogueSystemObj.AddComponent<DialogueRunner>();
            SerializedObject dialogueRunnerObject = new SerializedObject(dialogueRunner);
            dialogueRunnerObject.Update();

            YarnProject yarnProjectAsset = AssetDatabase.LoadAssetAtPath<YarnProject>(YarnProjectPath);
            if (yarnProjectAsset == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:YarnProject", new[] { "Assets/Resources/Yarn" });
                if (guids.Length > 0)
                {
                    yarnProjectAsset = AssetDatabase.LoadAssetAtPath<YarnProject>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
            }

            if (yarnProjectAsset != null)
            {
                dialogueRunnerObject.FindProperty("yarnProject").objectReferenceValue = yarnProjectAsset;
            }
            else
            {
                Debug.LogWarning($"{logPrefix}: YarnProject not found. Assign it manually in the Inspector.");
            }

            SerializedProperty startAutomaticallyProperty = dialogueRunnerObject.FindProperty("startAutomatically");
            if (startAutomaticallyProperty != null)
            {
                startAutomaticallyProperty.boolValue = false;
            }

            SerializedProperty startNodeProperty = dialogueRunnerObject.FindProperty("startNode");
            if (startNodeProperty != null)
            {
                startNodeProperty.stringValue = startNode;
            }

            dialogueRunnerObject.ApplyModifiedProperties();

            ChatDialogueView chatDialogueView = dialogueSystemObj.AddComponent<ChatDialogueView>();

            dialogueRunnerObject = new SerializedObject(dialogueRunner);
            dialogueRunnerObject.Update();
            SerializedProperty dialoguePresenters = dialogueRunnerObject.FindProperty("dialoguePresenters");
            if (dialoguePresenters != null)
            {
                dialoguePresenters.arraySize = 1;
                dialoguePresenters.GetArrayElementAtIndex(0).objectReferenceValue = chatDialogueView;
            }
            dialogueRunnerObject.ApplyModifiedProperties();

            ScenarioManager scenarioManager = dialogueSystemObj.AddComponent<ScenarioManager>();
            SerializedObject scenarioManagerObject = new SerializedObject(scenarioManager);
            scenarioManagerObject.Update();
            scenarioManagerObject.FindProperty("m_DialogueRunner").objectReferenceValue = dialogueRunner;
            scenarioManagerObject.FindProperty("m_ChatController").objectReferenceValue = chatController;
            scenarioManagerObject.FindProperty("m_StartNode").stringValue = startNode;
            scenarioManagerObject.FindProperty("m_AutoStartYarn").boolValue = true;
            scenarioManagerObject.ApplyModifiedProperties();

            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();

            EditorSceneManager.SaveScene(scene, scenePath);
            AddSceneToBuildSettings(scenePath);

            Debug.Log($"{logPrefix}: Scene saved to {scenePath}");
            Debug.Log($"{logPrefix}: Ready. Open the scene and press Play to preview {startNode}.");
        }

        private static GameObject CreateUIElement(string name, Transform parent)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static void StretchToFill(GameObject obj)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static TMP_InputField CreateInputField(Transform parent)
        {
            GameObject inputObj = CreateUIElement("InputField", parent);
            RectTransform inputTransform = inputObj.GetComponent<RectTransform>();
            inputTransform.anchorMin = Vector2.zero;
            inputTransform.anchorMax = Vector2.one;
            inputTransform.offsetMin = new Vector2(20f, 20f);
            inputTransform.offsetMax = new Vector2(-140f, -20f);

            Image inputBackground = inputObj.AddComponent<Image>();
            inputBackground.color = new Color(0.2f, 0.2f, 0.24f);

            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();

            GameObject textArea = CreateUIElement("Text Area", inputObj.transform);
            StretchToFill(textArea);
            textArea.AddComponent<RectMask2D>();

            GameObject textObj = CreateUIElement("Text", textArea.transform);
            StretchToFill(textObj);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.color = Color.white;
            text.fontSize = 28f;

            GameObject placeholderObj = CreateUIElement("Placeholder", textArea.transform);
            StretchToFill(placeholderObj);
            TextMeshProUGUI placeholder = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholder.text = "Preview only...";
            placeholder.color = new Color(0.5f, 0.5f, 0.5f);
            placeholder.fontSize = 28f;
            placeholder.fontStyle = FontStyles.Italic;

            inputField.textViewport = textArea.GetComponent<RectTransform>();
            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            return inputField;
        }

        private static Button CreateSendButton(Transform parent)
        {
            GameObject buttonObj = CreateUIElement("SendButton", parent);
            RectTransform buttonTransform = buttonObj.GetComponent<RectTransform>();
            buttonTransform.anchorMin = new Vector2(1f, 0f);
            buttonTransform.anchorMax = new Vector2(1f, 1f);
            buttonTransform.offsetMin = new Vector2(-120f, 20f);
            buttonTransform.offsetMax = new Vector2(-20f, -20f);

            Image buttonBackground = buttonObj.AddComponent<Image>();
            buttonBackground.color = new Color(0.2f, 0.5f, 1f);

            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonBackground;

            GameObject labelObj = CreateUIElement("Label", buttonObj.transform);
            StretchToFill(labelObj);
            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = "Send";
            label.color = Color.white;
            label.fontSize = 26f;
            label.alignment = TextAlignmentOptions.Center;

            return button;
        }

        private static GameObject CreateChoiceButtonPrefab(string logPrefix)
        {
            const string prefabPath = "Assets/Prefabs/UI/ChoiceButton.prefab";
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            GameObject buttonObj = new GameObject("ChoiceButton", typeof(RectTransform));

            Image buttonBackground = buttonObj.AddComponent<Image>();
            buttonBackground.color = new Color(0.25f, 0.45f, 0.8f);

            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonBackground;

            LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 60f;
            layoutElement.preferredHeight = 70f;

            GameObject labelObj = new GameObject("Label", typeof(RectTransform));
            labelObj.transform.SetParent(buttonObj.transform, false);
            RectTransform labelTransform = labelObj.GetComponent<RectTransform>();
            labelTransform.anchorMin = Vector2.zero;
            labelTransform.anchorMax = Vector2.one;
            labelTransform.offsetMin = new Vector2(20f, 0f);
            labelTransform.offsetMax = new Vector2(-20f, 0f);

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = "Choice";
            label.color = Color.white;
            label.fontSize = 24f;
            label.alignment = TextAlignmentOptions.Center;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(buttonObj, prefabPath);
            Object.DestroyImmediate(buttonObj);

            Debug.Log($"{logPrefix}: ChoiceButton prefab saved to {prefabPath}");
            return prefab;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool hasScene = false;

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (scene.path == scenePath)
                {
                    hasScene = true;
                    break;
                }
            }

            if (!hasScene)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }

            BuildSettingsHelper.EnsureCoreScenesInBuildSettings();
        }
    }
}
#endif
