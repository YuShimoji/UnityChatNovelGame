using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// セーブシステムUIのセットアップを自動化するエディタスクリプト
    /// </summary>
    public class SaveSystemUISetup : EditorWindow
    {
        [MenuItem("Project FoundPhone/Setup/Save System UI")]
        public static void ShowWindow()
        {
            GetWindow<SaveSystemUISetup>("Save UI Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Save System UI Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Save System UI Hierarchy"))
            {
                CreateUIHierarchy();
            }
        }

        private static void CreateUIHierarchy()
        {
            // 1. Canvasの取得または作成
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // 2. SaveLoadUIの作成
            GameObject saveLoadUIGO = new GameObject("SaveLoadUI", typeof(RectTransform));
            saveLoadUIGO.transform.SetParent(canvas.transform, false);
            SaveLoadUI saveLoadUI = saveLoadUIGO.AddComponent<SaveLoadUI>();
            RectTransform saveLoadRT = saveLoadUIGO.GetComponent<RectTransform>();
            saveLoadRT.anchorMin = Vector2.zero;
            saveLoadRT.anchorMax = Vector2.one;
            saveLoadRT.sizeDelta = Vector2.zero;

            // 3. Panelの作成
            GameObject panelGO = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelGO.transform.SetParent(saveLoadUIGO.transform, false);
            RectTransform panelRT = panelGO.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.1f, 0.1f);
            panelRT.anchorMax = new Vector2(0.9f, 0.9f);
            panelRT.sizeDelta = Vector2.zero;
            panelGO.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);

            // 4. TitleTextの作成
            GameObject titleGO = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleGO.transform.SetParent(panelGO.transform, false);
            TextMeshProUGUI titleText = titleGO.GetComponent<TextMeshProUGUI>();
            titleText.text = "Save Game";
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontSize = 36;
            RectTransform titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 0.9f);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.sizeDelta = Vector2.zero;

            // 5. SlotContainerの作成
            GameObject containerGO = new GameObject("SlotContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
            containerGO.transform.SetParent(panelGO.transform, false);
            VerticalLayoutGroup vlg = containerGO.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 10;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            RectTransform containerRT = containerGO.GetComponent<RectTransform>();
            containerRT.anchorMin = new Vector2(0.1f, 0.2f);
            containerRT.anchorMax = new Vector2(0.9f, 0.85f);
            containerRT.sizeDelta = Vector2.zero;

            // 6. CloseButtonの作成
            GameObject closeButtonGO = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
            closeButtonGO.transform.SetParent(panelGO.transform, false);
            RectTransform closeRT = closeButtonGO.GetComponent<RectTransform>();
            closeRT.anchorMin = new Vector2(0.4f, 0.05f);
            closeRT.anchorMax = new Vector2(0.6f, 0.15f);
            closeRT.sizeDelta = Vector2.zero;
            
            GameObject btnTextGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            btnTextGO.transform.SetParent(closeButtonGO.transform, false);
            TextMeshProUGUI btnText = btnTextGO.GetComponent<TextMeshProUGUI>();
            btnText.text = "Close";
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.black;
            btnText.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            btnText.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            btnText.GetComponent<RectTransform>().anchorMax = Vector2.one;

            // 7. SaveSlotUI プレハブのスケルトン作成
            GameObject slotPrefabGO = CreateSlotPrefabSkeleton();
            
            // 8. SaveLoadUIの参照設定
            SerializedObject so = new SerializedObject(saveLoadUI);
            so.FindProperty("m_Panel").objectReferenceValue = panelGO;
            so.FindProperty("m_SlotContainer").objectReferenceValue = containerGO.transform;
            so.FindProperty("m_CloseButton").objectReferenceValue = closeButtonGO.GetComponent<Button>();
            so.FindProperty("m_TitleText").objectReferenceValue = titleText;
            so.FindProperty("m_SlotPrefab").objectReferenceValue = slotPrefabGO.GetComponent<SaveSlotUI>();
            so.ApplyModifiedProperties();

            Selection.activeGameObject = saveLoadUIGO;
            Debug.Log("Save System UI Hierarchy created. Please save the 'SaveSlotUI_Template' as a prefab and assign it to the SaveLoadUI component.");
        }

        private static GameObject CreateSlotPrefabSkeleton()
        {
            GameObject slotGO = new GameObject("SaveSlotUI_Template", typeof(RectTransform), typeof(SaveSlotUI));
            RectTransform slotRT = slotGO.GetComponent<RectTransform>();
            slotRT.sizeDelta = new Vector2(400, 100);

            // MainButton
            GameObject mainBtnGO = new GameObject("MainButton", typeof(RectTransform), typeof(Image), typeof(Button));
            mainBtnGO.transform.SetParent(slotGO.transform, false);
            RectTransform mainRT = mainBtnGO.GetComponent<RectTransform>();
            mainRT.anchorMin = Vector2.zero;
            mainRT.anchorMax = Vector2.one;
            mainRT.sizeDelta = Vector2.zero;

            // SlotNumberText
            GameObject numGO = new GameObject("SlotNumberText", typeof(RectTransform), typeof(TextMeshProUGUI));
            numGO.transform.SetParent(mainBtnGO.transform, false);
            TextMeshProUGUI numText = numGO.GetComponent<TextMeshProUGUI>();
            numText.text = "Slot 1";
            numText.fontSize = 24;
            numText.color = Color.black;
            RectTransform numRT = numGO.GetComponent<RectTransform>();
            numRT.anchorMin = new Vector2(0, 0.5f);
            numRT.anchorMax = new Vector2(0.3f, 1);
            numRT.sizeDelta = Vector2.zero;

            // SaveDataPanel
            GameObject dataPanelGO = new GameObject("SaveDataPanel", typeof(RectTransform));
            dataPanelGO.transform.SetParent(mainBtnGO.transform, false);
            RectTransform dataRT = dataPanelGO.GetComponent<RectTransform>();
            dataRT.anchorMin = new Vector2(0.3f, 0);
            dataRT.anchorMax = new Vector2(1, 1);
            dataRT.sizeDelta = Vector2.zero;

            // SaveInfoText
            GameObject infoGO = new GameObject("SaveInfoText", typeof(RectTransform), typeof(TextMeshProUGUI));
            infoGO.transform.SetParent(dataPanelGO.transform, false);
            TextMeshProUGUI infoText = infoGO.GetComponent<TextMeshProUGUI>();
            infoText.text = "Date - Topics";
            infoText.fontSize = 18;
            infoText.color = Color.black;
            infoText.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            infoText.GetComponent<RectTransform>().anchorMax = Vector2.one;
            infoText.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            // EmptySlotPanel
            GameObject emptyPanelGO = new GameObject("EmptySlotPanel", typeof(RectTransform));
            emptyPanelGO.transform.SetParent(mainBtnGO.transform, false);
            RectTransform emptyRT = emptyPanelGO.GetComponent<RectTransform>();
            emptyRT.anchorMin = new Vector2(0.3f, 0);
            emptyRT.anchorMax = new Vector2(1, 1);
            emptyRT.sizeDelta = Vector2.zero;

            // EmptySlotText
            GameObject emptyTextGO = new GameObject("EmptySlotText", typeof(RectTransform), typeof(TextMeshProUGUI));
            emptyTextGO.transform.SetParent(emptyPanelGO.transform, false);
            TextMeshProUGUI emptyText = emptyTextGO.GetComponent<TextMeshProUGUI>();
            emptyText.text = "Empty Slot";
            emptyText.fontSize = 18;
            emptyText.color = Color.gray;
            emptyText.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            emptyText.GetComponent<RectTransform>().anchorMax = Vector2.one;
            emptyText.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            // DeleteButton
            GameObject delBtnGO = new GameObject("DeleteButton", typeof(RectTransform), typeof(Image), typeof(Button));
            delBtnGO.transform.SetParent(slotGO.transform, false);
            RectTransform delRT = delBtnGO.GetComponent<RectTransform>();
            delRT.anchorMin = new Vector2(0.85f, 0.1f);
            delRT.anchorMax = new Vector2(0.95f, 0.9f);
            delRT.sizeDelta = Vector2.zero;
            delBtnGO.GetComponent<Image>().color = Color.red;

            // Setup SaveSlotUI
            SaveSlotUI slotUI = slotGO.GetComponent<SaveSlotUI>();
            SerializedObject so = new SerializedObject(slotUI);
            so.FindProperty("m_MainButton").objectReferenceValue = mainBtnGO.GetComponent<Button>();
            so.FindProperty("m_DeleteButton").objectReferenceValue = delBtnGO.GetComponent<Button>();
            so.FindProperty("m_SlotNumberText").objectReferenceValue = numText;
            so.FindProperty("m_SaveInfoText").objectReferenceValue = infoText;
            so.FindProperty("m_EmptySlotText").objectReferenceValue = emptyText;
            so.FindProperty("m_SaveDataPanel").objectReferenceValue = dataPanelGO;
            so.FindProperty("m_EmptySlotPanel").objectReferenceValue = emptyPanelGO;
            so.ApplyModifiedProperties();

            return slotGO;
        }
    }
}
