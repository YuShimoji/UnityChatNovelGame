#if YARN_SPINNER
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Core;
using Yarn.Unity;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// Vertical Slice 用 DebugChatScene のセットアップスクリプト。
    /// Menu: Tools/Setup Vertical Slice Scene
    /// YARN_SPINNER シンボルが有効な場合のみコンパイルされる。
    /// </summary>
    public static class VerticalSliceSceneSetup
    {
        [MenuItem("Tools/Setup Vertical Slice Scene")]
        public static void SetupScene()
        {
            // 新しいシーンを作成
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            GameObject cameraObj = new GameObject("Main Camera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.12f, 0.15f);
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();

            // EventSystem
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // Background
            GameObject bgObj = CreateUIElement("Background", canvasObj.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.13f);
            StretchToFill(bgObj);

            // === Chat UI ===
            // ScrollView
            GameObject scrollViewObj = CreateUIElement("ChatScrollView", canvasObj.transform);
            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            RectTransform scrollRT = scrollViewObj.GetComponent<RectTransform>();
            scrollRT.anchorMin = Vector2.zero;
            scrollRT.anchorMax = Vector2.one;
            scrollRT.offsetMin = new Vector2(0, 150);
            scrollRT.offsetMax = Vector2.zero;

            // Viewport
            GameObject viewportObj = CreateUIElement("Viewport", scrollViewObj.transform);
            StretchToFill(viewportObj);
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;
            Image vpImg = viewportObj.AddComponent<Image>();
            vpImg.color = new Color(1, 1, 1, 0.01f);

            // Content
            GameObject contentObj = CreateUIElement("Content", viewportObj.transform);
            RectTransform contentRT = contentObj.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.sizeDelta = new Vector2(0, 0);

            VerticalLayoutGroup vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(20, 20, 20, 20);
            vlg.spacing = 16;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;

            ContentSizeFitter csf = contentObj.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRT;
            scrollRect.viewport = viewportObj.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;

            // TypingIndicator
            GameObject typingInstance = null;
            GameObject typingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/TypingIndicator.prefab");
            if (typingPrefab != null)
            {
                typingInstance = PrefabUtility.InstantiatePrefab(typingPrefab, contentObj.transform) as GameObject;
                typingInstance.SetActive(false);
            }

            // Choice Container
            GameObject choiceContainerObj = CreateUIElement("ChoiceContainer", canvasObj.transform);
            RectTransform choiceRT = choiceContainerObj.GetComponent<RectTransform>();
            choiceRT.anchorMin = new Vector2(0.05f, 0.3f);
            choiceRT.anchorMax = new Vector2(0.95f, 0.7f);
            choiceRT.offsetMin = Vector2.zero;
            choiceRT.offsetMax = Vector2.zero;

            VerticalLayoutGroup choiceVLG = choiceContainerObj.AddComponent<VerticalLayoutGroup>();
            choiceVLG.spacing = 12;
            choiceVLG.childControlHeight = true;
            choiceVLG.childControlWidth = true;
            choiceVLG.childForceExpandHeight = false;
            choiceVLG.childForceExpandWidth = true;
            choiceVLG.padding = new RectOffset(20, 20, 20, 20);
            choiceContainerObj.SetActive(false);

            // ChoiceButton Prefab
            GameObject choiceButtonTemplate = CreateChoiceButtonPrefab();

            // Footer (Input + Send)
            GameObject footerObj = CreateUIElement("Footer", canvasObj.transform);
            RectTransform footerRT = footerObj.GetComponent<RectTransform>();
            footerRT.anchorMin = Vector2.zero;
            footerRT.anchorMax = new Vector2(1, 0);
            footerRT.offsetMin = Vector2.zero;
            footerRT.offsetMax = new Vector2(0, 150);
            Image footerBg = footerObj.AddComponent<Image>();
            footerBg.color = new Color(0.15f, 0.15f, 0.18f);

            // Input Field
            TMP_InputField inputField = CreateInputField(footerObj.transform);

            // Send Button
            Button sendButton = CreateSendButton(footerObj.transform);

            // === ChatController ===
            ChatController chatController = scrollViewObj.AddComponent<ChatController>();
            SerializedObject chatSO = new SerializedObject(chatController);
            chatSO.Update();
            chatSO.FindProperty("m_ScrollRect").objectReferenceValue = scrollRect;
            chatSO.FindProperty("m_LayoutGroup").objectReferenceValue = vlg;
            chatSO.FindProperty("m_InputField").objectReferenceValue = inputField;
            chatSO.FindProperty("m_SendButton").objectReferenceValue = sendButton;
            chatSO.FindProperty("m_ChoiceContainer").objectReferenceValue = choiceContainerObj.transform;
            chatSO.FindProperty("m_ChoiceButtonPrefab").objectReferenceValue = choiceButtonTemplate;

            GameObject bubblePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/MessageBubble.prefab");
            if (bubblePrefab != null)
                chatSO.FindProperty("m_MessageBubblePrefab").objectReferenceValue = bubblePrefab;
            else
                Debug.LogError("VerticalSliceSetup: MessageBubble.prefab not found!");

            if (typingInstance != null)
                chatSO.FindProperty("m_TypingIndicator").objectReferenceValue = typingInstance;

            chatSO.ApplyModifiedProperties();

            // === Dialogue System ===
            GameObject dialogueSystemObj = new GameObject("DialogueSystem");

            // InMemoryVariableStorage
            InMemoryVariableStorage variableStorage = dialogueSystemObj.AddComponent<InMemoryVariableStorage>();

            // DialogueRunner
            DialogueRunner dialogueRunner = dialogueSystemObj.AddComponent<DialogueRunner>();
            SerializedObject drSO = new SerializedObject(dialogueRunner);
            drSO.Update();

            // YarnProject 参照
            YarnProject yarnProjectAsset = AssetDatabase.LoadAssetAtPath<YarnProject>("Assets/Resources/Yarn/Project.asset");
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
                drSO.FindProperty("yarnProject").objectReferenceValue = yarnProjectAsset;
            }
            else
            {
                Debug.LogWarning("VerticalSliceSetup: YarnProject not found. Assign manually in Inspector.");
            }

            drSO.FindProperty("startAutomatically").boolValue = false;
            drSO.ApplyModifiedProperties();

            // ChatDialogueView
            ChatDialogueView chatDialogueView = dialogueSystemObj.AddComponent<ChatDialogueView>();

            // DialoguePresenters に ChatDialogueView を登録
            drSO = new SerializedObject(dialogueRunner);
            drSO.Update();
            SerializedProperty dialoguePresenters = drSO.FindProperty("dialoguePresenters");
            if (dialoguePresenters != null)
            {
                dialoguePresenters.arraySize = 1;
                dialoguePresenters.GetArrayElementAtIndex(0).objectReferenceValue = chatDialogueView;
            }
            drSO.ApplyModifiedProperties();

            // ScenarioManager
            ScenarioManager scenarioManager = dialogueSystemObj.AddComponent<ScenarioManager>();
            SerializedObject smSO = new SerializedObject(scenarioManager);
            smSO.Update();
            smSO.FindProperty("m_DialogueRunner").objectReferenceValue = dialogueRunner;
            smSO.FindProperty("m_ChatController").objectReferenceValue = chatController;
            smSO.FindProperty("m_StartNode").stringValue = "Start";
            smSO.FindProperty("m_AutoStartYarn").boolValue = true;
            smSO.ApplyModifiedProperties();

            // === SaveManager ===
            GameObject saveManagerObj = new GameObject("SaveManager");
            saveManagerObj.AddComponent<SaveManager>();

            // シーンを保存
            string scenePath = "Assets/Scenes/DebugChatScene.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"VerticalSliceSetup: Scene saved to {scenePath}");
            Debug.Log("VerticalSliceSetup: Complete! Please verify:");
            Debug.Log("  1. DialogueRunner の YarnProject 参照が正しいこと");
            Debug.Log("  2. Play ボタンで Start ノードから進行すること");

            // Build Settings にシーンを追加
            AddSceneToBuildSettings(scenePath);
        }

        private static GameObject CreateUIElement(string name, Transform parent)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static void StretchToFill(GameObject obj)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static TMP_InputField CreateInputField(Transform parent)
        {
            GameObject inputObj = CreateUIElement("InputField", parent);
            RectTransform inputRT = inputObj.GetComponent<RectTransform>();
            inputRT.anchorMin = Vector2.zero;
            inputRT.anchorMax = Vector2.one;
            inputRT.offsetMin = new Vector2(20, 20);
            inputRT.offsetMax = new Vector2(-140, -20);

            Image inputBg = inputObj.AddComponent<Image>();
            inputBg.color = new Color(0.2f, 0.2f, 0.24f);

            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();

            GameObject textArea = CreateUIElement("Text Area", inputObj.transform);
            StretchToFill(textArea);
            textArea.AddComponent<RectMask2D>();

            GameObject textObj = CreateUIElement("Text", textArea.transform);
            StretchToFill(textObj);
            TextMeshProUGUI textTmp = textObj.AddComponent<TextMeshProUGUI>();
            textTmp.color = Color.white;
            textTmp.fontSize = 28;

            GameObject placeholderObj = CreateUIElement("Placeholder", textArea.transform);
            StretchToFill(placeholderObj);
            TextMeshProUGUI placeholderTmp = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderTmp.text = "メッセージを入力...";
            placeholderTmp.color = new Color(0.5f, 0.5f, 0.5f);
            placeholderTmp.fontSize = 28;
            placeholderTmp.fontStyle = FontStyles.Italic;

            inputField.textViewport = textArea.GetComponent<RectTransform>();
            inputField.textComponent = textTmp;
            inputField.placeholder = placeholderTmp;

            return inputField;
        }

        private static Button CreateSendButton(Transform parent)
        {
            GameObject btnObj = CreateUIElement("SendButton", parent);
            RectTransform btnRT = btnObj.GetComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(1, 0);
            btnRT.anchorMax = new Vector2(1, 1);
            btnRT.offsetMin = new Vector2(-120, 20);
            btnRT.offsetMax = new Vector2(-20, -20);

            Image btnBg = btnObj.AddComponent<Image>();
            btnBg.color = new Color(0.2f, 0.5f, 1.0f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnBg;

            GameObject labelObj = CreateUIElement("Label", btnObj.transform);
            StretchToFill(labelObj);
            TextMeshProUGUI labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = "送信";
            labelTmp.color = Color.white;
            labelTmp.fontSize = 26;
            labelTmp.alignment = TextAlignmentOptions.Center;

            return btn;
        }

        private static GameObject CreateChoiceButtonPrefab()
        {
            GameObject btnObj = new GameObject("ChoiceButton", typeof(RectTransform));

            Image btnBg = btnObj.AddComponent<Image>();
            btnBg.color = new Color(0.25f, 0.45f, 0.8f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnBg;

            LayoutElement le = btnObj.AddComponent<LayoutElement>();
            le.minHeight = 60;
            le.preferredHeight = 70;

            GameObject labelObj = new GameObject("Label", typeof(RectTransform));
            labelObj.transform.SetParent(btnObj.transform, false);
            RectTransform labelRT = labelObj.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(20, 0);
            labelRT.offsetMax = new Vector2(-20, 0);

            TextMeshProUGUI labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = "Choice";
            labelTmp.color = Color.white;
            labelTmp.fontSize = 24;
            labelTmp.alignment = TextAlignmentOptions.Center;

            string prefabPath = "Assets/Prefabs/UI/ChoiceButton.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(btnObj, prefabPath);
            Object.DestroyImmediate(btnObj);
            Debug.Log($"VerticalSliceSetup: ChoiceButton prefab saved to {prefabPath}");
            return prefab;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            bool hasTitleScene = false;
            bool hasDebugScene = false;
            foreach (var s in scenes)
            {
                if (s.path.Contains("TitleScene")) hasTitleScene = true;
                if (s.path == scenePath) hasDebugScene = true;
            }

            if (!hasTitleScene)
            {
                string titlePath = "Assets/Scenes/TitleScene.unity";
                scenes.Insert(0, new EditorBuildSettingsScene(titlePath, true));
            }

            if (!hasDebugScene)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("VerticalSliceSetup: Build Settings updated with TitleScene and DebugChatScene.");
        }
    }
}
#endif
