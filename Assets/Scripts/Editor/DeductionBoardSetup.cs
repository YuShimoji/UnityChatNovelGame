using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro; // Assuming TextMeshPro is used as per TopicCard.cs
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.EditorTools
{
    public class DeductionBoardSetup : EditorWindow
    {
        [MenuItem("Project FoundPhone/Setup Deduction Board")]
        public static void Setup()
        {
            CreateTopicCardPrefab();
            CreateDeductionBoardPrefab();
            UnityEngine.Debug.Log("Deduction Board Setup Complete!");
        }

        private static void CreateTopicCardPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/TopicCard.prefab";
            
            // Check if prefab exists
             if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                UnityEngine.Debug.Log($"TopicCard prefab already exists at {prefabPath}");
                return;
            }

            // Create GameObject structure
            GameObject go = new GameObject("TopicCard", typeof(RectTransform));
            go.AddComponent<Image>(); // Background
            go.AddComponent<Button>();
            go.AddComponent<TopicCard>();

            // Icon
            GameObject iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconGo.transform.SetParent(go.transform, false);
            
            // Title
            GameObject titleGo = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleGo.transform.SetParent(go.transform, false);

            // Assign references
            TopicCard card = go.GetComponent<TopicCard>();
            // Use SerializedObject to assign private fields
            SerializedObject so = new SerializedObject(card);
            so.FindProperty("m_IconImage").objectReferenceValue = iconGo.GetComponent<Image>();
            so.FindProperty("m_TitleText").objectReferenceValue = titleGo.GetComponent<TextMeshProUGUI>();
            // m_Button does not exist in TopicCard, removing assignment
            so.ApplyModifiedProperties();

            // Save as Prefab
            // Try/Catch to handle file locks gracefully
            try
            {
                if (!System.IO.Directory.Exists("Assets/Prefabs/UI"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
                }
                
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                UnityEngine.Debug.Log($"[Success] Created TopicCard prefab at {prefabPath}");
                
                // Optional: Destroy only if successful
                // DestroyImmediate(go); 
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[Error] Failed to save Prefab: {e.Message}. The object 'TopicCard' remains in the scene.");
            }
            Selection.activeGameObject = go;
        }

        private static void CreateDeductionBoardPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/DeductionBoard.prefab";

             if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                 UnityEngine.Debug.Log($"DeductionBoard prefab already exists at {prefabPath}");
                return;
            }

            // Main Board
            GameObject go = new GameObject("DeductionBoard", typeof(RectTransform));
            go.AddComponent<Canvas>(); // Optional, but useful if it's a separate UI root
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            go.AddComponent<DeductionBoard>();

            // Panel (Background)
            GameObject panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(go.transform, false);
            // Full stretch
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            // Scroll View structure
            GameObject scrollView = new GameObject("ScrollView", typeof(RectTransform));
            scrollView.transform.SetParent(panel.transform, false);
            // Add ScrollRect, Mask, etc. - Simplifying for now, just the container
            
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform));
            viewport.transform.SetParent(scrollView.transform, false);

            GameObject content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            content.AddComponent<GridLayoutGroup>();

            // Assign References
            DeductionBoard board = go.GetComponent<DeductionBoard>();
            SerializedObject so = new SerializedObject(board);
            // m_Panel does not exist in DeductionBoard, removing assignment
            so.FindProperty("m_CardContainer").objectReferenceValue = content.transform; // Renamed from m_TopicContainer
            
            // Assign TopicCard Prefab
            GameObject cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/TopicCard.prefab");
            if (cardPrefab != null)
            {
                so.FindProperty("m_TopicCardPrefab").objectReferenceValue = cardPrefab.GetComponent<TopicCard>();
            }

            so.ApplyModifiedProperties();

            // Save as Prefab
            // Try/Catch to handle file locks gracefully
            try
            {
                if (!System.IO.Directory.Exists("Assets/Prefabs/UI"))
                {
                    System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
                }
                
                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                UnityEngine.Debug.Log($"[Success] Created DeductionBoard prefab at {prefabPath}");
                
                // Optional: Destroy only if successful, but for now let's keep it to be sure
                // DestroyImmediate(go); 
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"[Error] Failed to save Prefab: {e.Message}. The object 'DeductionBoard' remains in the scene. Please drag it to 'Assets/Prefabs/UI/' manually.");
            }
            
            // Select it so the user sees it
            Selection.activeGameObject = go;
        }
    }
}
