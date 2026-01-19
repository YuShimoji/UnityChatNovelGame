using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.Editor
{
    public class RefineChatUI : EditorWindow
    {
        [MenuItem("Tools/Refine Chat UI")]
        public static void CreateChatDevScene()
        {
            // 1. Scene Creation
            string scenePath = "Assets/Scenes/ChatDevScene.unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Set up Main Camera
            GameObject cameraObj = new GameObject("Main Camera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.95f, 0.95f, 0.95f);
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();

            // Set up EventSystem
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // 2. UI Setup (Canvas)
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // Portrait
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasObj.transform, false);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.9f, 0.9f, 0.9f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // ScrollView Area
            GameObject scrollViewObj = new GameObject("ChatScrollView");
            scrollViewObj.transform.SetParent(canvasObj.transform, false);
            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            RectTransform scrollRectTrans = scrollViewObj.GetComponent<RectTransform>();
            scrollRectTrans.anchorMin = Vector2.zero;
            scrollRectTrans.anchorMax = Vector2.one;
            scrollRectTrans.offsetMin = new Vector2(0, 150); // Leave space for footer
            scrollRectTrans.offsetMax = new Vector2(0, 0);

            // Viewport
            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollViewObj.transform, false);
            RectTransform viewRect = viewportObj.AddComponent<RectTransform>();
            viewRect.anchorMin = Vector2.zero;
            viewRect.anchorMax = Vector2.one;
            viewRect.sizeDelta = Vector2.zero;
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;
            Image viewImage = viewportObj.AddComponent<Image>();
            viewImage.color = new Color(1, 1, 1, 0.01f); // Transparent but raycastable

            // Content
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 300); // Initial height
            
            VerticalLayoutGroup vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(20, 20, 20, 20);
            vlg.spacing = 20;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;

            ContentSizeFitter csf = contentObj.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRect;
            scrollRect.viewport = viewRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;


            // Footer Area
            GameObject footerObj = new GameObject("Footer");
            footerObj.transform.SetParent(canvasObj.transform, false);
            RectTransform footerRect = footerObj.AddComponent<RectTransform>();
            footerRect.anchorMin = Vector2.zero;
            footerRect.anchorMax = new Vector2(1, 0);
            footerRect.offsetMin = new Vector2(0, 0);
            footerRect.offsetMax = new Vector2(0, 150);
            Image footerBg = footerObj.AddComponent<Image>();
            footerBg.color = Color.white;

            // Input Field
            GameObject inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(footerObj.transform, false);
            RectTransform inputTrans = inputObj.AddComponent<RectTransform>();
            inputTrans.anchorMin = Vector2.zero;
            inputTrans.anchorMax = Vector2.one;
            inputTrans.offsetMin = new Vector2(20, 20);
            inputTrans.offsetMax = new Vector2(-150, -20);
            
            Image inputImage = inputObj.AddComponent<Image>();
            inputImage.color = new Color(0.95f, 0.95f, 0.95f);
            
            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
            
            GameObject textArea = new GameObject("TextArea");
            textArea.transform.SetParent(inputObj.transform, false);
            RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 0);
            textAreaRect.offsetMax = new Vector2(-10, 0);
            
            GameObject viewPortInput = new GameObject("TextViewport");
            viewPortInput.transform.SetParent(textArea.transform, false);
            RectTransform vpInputRect = viewPortInput.AddComponent<RectTransform>();
            vpInputRect.anchorMin = Vector2.zero;
            vpInputRect.anchorMax = Vector2.one;
            viewPortInput.AddComponent<RectMask2D>();
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(viewPortInput.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            
            TextMeshProUGUI textTmp = textObj.AddComponent<TextMeshProUGUI>();
            textTmp.color = Color.black;
            textTmp.fontSize = 32;
            
             // Placeholder
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(viewPortInput.transform, false);
            RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            TextMeshProUGUI placeholderTmp = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderTmp.text = "Enter message...";
            placeholderTmp.color = new Color(0.5f, 0.5f, 0.5f);
            placeholderTmp.fontSize = 32;
            placeholderTmp.fontStyle = FontStyles.Italic;

            inputField.textViewport = vpInputRect;
            inputField.textComponent = textTmp;
            inputField.placeholder = placeholderTmp;


            // Send Button
            GameObject btnObj = new GameObject("SendButton");
            btnObj.transform.SetParent(footerObj.transform, false);
            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 0);
            btnRect.anchorMax = new Vector2(1, 1);
            btnRect.offsetMin = new Vector2(-130, 20);
            btnRect.offsetMax = new Vector2(-20, -20);
            
            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.6f, 1.0f);
            
            Button btn = btnObj.AddComponent<Button>();
            
            GameObject btnTextObj = new GameObject("Text");
            btnTextObj.transform.SetParent(btnObj.transform, false);
            RectTransform btnTextRect = btnTextObj.AddComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            TextMeshProUGUI btnTmp = btnTextObj.AddComponent<TextMeshProUGUI>();
            btnTmp.text = "Send";
            btnTmp.color = Color.white;
            btnTmp.fontSize = 32;
            btnTmp.alignment = TextAlignmentOptions.Center;

            
            // 3. Controller Logic
            GameObject systemObj = new GameObject("ChatSystem");
            ChatController chatController = systemObj.AddComponent<ChatController>();
            
            // Assign References
            // Using SerializedObject/SerializedProperty to assign private fields
            SerializedObject so = new SerializedObject(chatController);
            so.Update();
            
            so.FindProperty("m_ScrollRect").objectReferenceValue = scrollRect;
            so.FindProperty("m_LayoutGroup").objectReferenceValue = vlg;
            so.FindProperty("m_InputField").objectReferenceValue = inputField;
            so.FindProperty("m_SendButton").objectReferenceValue = btn;
            
            // Load Prefabs
            GameObject bubblePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/MessageBubble.prefab");
            GameObject typingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/TypingIndicator.prefab");
            
            if (bubblePrefab != null) so.FindProperty("m_MessageBubblePrefab").objectReferenceValue = bubblePrefab;
            else Debug.LogError("MessageBubble prefab not found at Assets/Prefabs/UI/MessageBubble.prefab");
            
            if (typingPrefab != null) {
                 // Instantiate typing indicator as child of content
                 GameObject typingInstance = PrefabUtility.InstantiatePrefab(typingPrefab, contentObj.transform) as GameObject;
                 typingInstance.SetActive(false);
                 so.FindProperty("m_TypingIndicator").objectReferenceValue = typingInstance;
            }
            else Debug.LogError("TypingIndicator prefab not found at Assets/Prefabs/UI/TypingIndicator.prefab");
            
            so.ApplyModifiedProperties();

            // Save and Open
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"Created new scene at {scenePath}");
        }
    }
}
