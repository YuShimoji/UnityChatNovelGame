using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Yarn.Unity;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Core;
using System.Reflection;
using System;
using System.Linq;

namespace ProjectFoundPhone.Logging.Editor
{
    public class DebugSceneBuilder : EditorWindow
    {
        [MenuItem("Tools/FoundPhone/Setup Debug Scene")]
        public static void SetupScene()
        {
            // 1. Setup Canvas and EventSystem
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                canvasObj.layer = LayerMask.NameToLayer("UI");
                
                Canvas c = canvasObj.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                canvasObj.AddComponent<GraphicRaycaster>();
            }
            Canvas canvas = canvasObj.GetComponent<Canvas>();

            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // 2. Setup Chat UI
            // 既存のChatRootを削除（TextMesh ProのサンプルChatControllerも含む）
            GameObject chatRoot = GameObject.Find("ChatRoot");
            if (chatRoot != null)
            {
                // TextMesh ProのサンプルChatControllerがアタッチされている可能性があるため、すべてのコンポーネントを確認
                Component[] components = chatRoot.GetComponents<Component>();
                foreach (Component comp in components)
                {
                    if (comp != null && comp.GetType().Name == "ChatController")
                    {
                        // TextMesh ProのサンプルChatControllerかどうかを確認
                        string scriptGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                        if (scriptGuid == "53d91f98a2664f5cb9af11de72ac54ec")
                        {
                            UnityEngine.Debug.LogWarning("DebugSceneBuilder: Found TextMesh Pro sample ChatController, removing it.");
                            DestroyImmediate(comp);
                        }
                    }
                }
                DestroyImmediate(chatRoot);
            }
            
            chatRoot = new GameObject("ChatRoot", typeof(RectTransform));
            chatRoot.transform.SetParent(canvas.transform, false);
            StretchToFill(chatRoot.GetComponent<RectTransform>());

            // Scroll View structure
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            viewport.transform.SetParent(chatRoot.transform, false);
            StretchToFill(viewport.GetComponent<RectTransform>());
            viewport.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.5f); // Semi-transparent background

            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.sizeDelta = new Vector2(0, 300); // Initial height
            contentRT.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.spacing = 10;
            vlg.padding = new RectOffset(20, 20, 20, 20);

            ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Chat Controller
            ScrollRect scrollRect = chatRoot.AddComponent<ScrollRect>();
            scrollRect.content = content.GetComponent<RectTransform>();
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.scrollSensitivity = 20;

            // 明示的にProjectFoundPhone.UI.ChatControllerを追加
            // TextMesh ProのサンプルChatControllerと区別するため、型を明示的に指定
            ProjectFoundPhone.UI.ChatController chatController = chatRoot.AddComponent<ProjectFoundPhone.UI.ChatController>();
            
            if (chatController == null)
            {
                UnityEngine.Debug.LogError("DebugSceneBuilder: Failed to add ChatController component.");
                return;
            }

            // 型の確認: 正しいChatControllerが取得されていることを検証
            Type chatControllerType = chatController.GetType();
            string expectedTypeName = typeof(ProjectFoundPhone.UI.ChatController).FullName;
            if (chatControllerType.FullName != expectedTypeName)
            {
                UnityEngine.Debug.LogError($"DebugSceneBuilder: Wrong ChatController type detected. Expected: {expectedTypeName}, Got: {chatControllerType.FullName}");
                return;
            }
            UnityEngine.Debug.Log($"DebugSceneBuilder: Correct ChatController type confirmed: {chatControllerType.FullName}");

            // リフレクションを使用して[SerializeField] privateフィールドを直接設定
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

            // デバッグ: 利用可能なフィールドをログ出力
            FieldInfo[] allFields = chatControllerType.GetFields(flags);
            var fieldNames = new System.Collections.Generic.List<string>();
            var allFieldInfo = new System.Collections.Generic.Dictionary<string, FieldInfo>();
            foreach (var field in allFields)
            {
                var serializeFieldAttr = field.GetCustomAttribute<SerializeField>();
                if (serializeFieldAttr != null || field.IsPublic)
                {
                    fieldNames.Add($"{field.Name} ({field.FieldType.Name})");
                    allFieldInfo[field.Name] = field;
                }
            }
            UnityEngine.Debug.Log($"DebugSceneBuilder: Available fields in ChatController: {string.Join(", ", fieldNames)}");

            // ヘルパー関数: フィールドを設定する
            bool SetFieldValue(string fieldName, object value, string fieldDescription)
            {
                if (allFieldInfo.TryGetValue(fieldName, out FieldInfo fieldInfo))
                {
                    try
                    {
                        fieldInfo.SetValue(chatController, value);
                        UnityEngine.Debug.Log($"DebugSceneBuilder: Successfully set {fieldName} via reflection");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"DebugSceneBuilder: Failed to set {fieldName} via reflection. Exception: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"DebugSceneBuilder: Failed to find {fieldName} field via reflection. Available fields: {string.Join(", ", fieldNames)}");
                    return false;
                }
            }

            // Set ScrollRect
            if (!SetFieldValue("m_ScrollRect", scrollRect, "ScrollRect"))
            {
                UnityEngine.Debug.LogError("DebugSceneBuilder: Critical error - m_ScrollRect field not found. ChatController may not function correctly.");
            }

            // Set LayoutGroup
            if (!SetFieldValue("m_LayoutGroup", vlg, "VerticalLayoutGroup"))
            {
                UnityEngine.Debug.LogError("DebugSceneBuilder: Critical error - m_LayoutGroup field not found. ChatController may not function correctly.");
            }
            
            // Load Prefabs
            GameObject msgPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/MessageBubble.prefab");
            GameObject typingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/TypingIndicator.prefab");
            
            if (msgPrefab != null)
            {
                if (!SetFieldValue("m_MessageBubblePrefab", msgPrefab, "MessageBubble Prefab"))
                {
                    UnityEngine.Debug.LogError("DebugSceneBuilder: Critical error - m_MessageBubblePrefab field not found. ChatController may not function correctly.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("MessageBubble prefab not found at Assets/Prefabs/UI/MessageBubble.prefab");
            }

            if (typingPrefab != null)
            {
                GameObject typingInstance = (GameObject)PrefabUtility.InstantiatePrefab(typingPrefab, content.transform);
                typingInstance.name = "TypingIndicator";
                typingInstance.SetActive(false);
                
                if (!SetFieldValue("m_TypingIndicator", typingInstance, "TypingIndicator GameObject"))
                {
                    UnityEngine.Debug.LogError("DebugSceneBuilder: Critical error - m_TypingIndicator field not found. ChatController may not function correctly.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("TypingIndicator prefab not found at Assets/Prefabs/UI/TypingIndicator.prefab");
            }

            // 変更をUnityエディタに通知
            EditorUtility.SetDirty(chatController);

            // 3. Setup Managers
            GameObject managerObj = GameObject.Find("GameManager");
            if (managerObj != null) DestroyImmediate(managerObj);
            managerObj = new GameObject("GameManager");

            ScenarioManager scenarioManager = managerObj.AddComponent<ScenarioManager>();
            DialogueRunner dialogueRunner = managerObj.AddComponent<DialogueRunner>();
            managerObj.AddComponent<InMemoryVariableStorage>();
            
            // Configure DialogueRunner
            if (dialogueRunner == null)
            {
                UnityEngine.Debug.LogError("DebugSceneBuilder: DialogueRunner component is null. Cannot configure.");
                return;
            }

            SerializedObject soRunner = new SerializedObject(dialogueRunner);
            if (soRunner == null)
            {
                UnityEngine.Debug.LogError("DebugSceneBuilder: Failed to create SerializedObject for DialogueRunner.");
                return;
            }

            string yarnProjectPath = "Assets/Resources/Yarn/Project.yarnproject";
            YarnProject yarnProject = AssetDatabase.LoadAssetAtPath<YarnProject>(yarnProjectPath);
            bool yarnProjectValid = false;
            
            if (yarnProject != null)
            {
                SerializedProperty yarnProjectProp = soRunner.FindProperty("yarnProject");
                if (yarnProjectProp != null)
                {
                    yarnProjectProp.objectReferenceValue = yarnProject;
                    soRunner.ApplyModifiedProperties();
                    
                    // YarnProjectが正しくコンパイルされているか確認
                    // リフレクションを使用してProgramプロパティを確認
                    try
                    {
                        Type yarnProjectType = yarnProject.GetType();
                        PropertyInfo programProp = yarnProjectType.GetProperty("Program");
                        if (programProp != null)
                        {
                            object program = programProp.GetValue(yarnProject);
                            if (program != null)
                            {
                                // Programが存在する場合、ノードが含まれているか確認
                                Type programType = program.GetType();
                                PropertyInfo nodesProp = programType.GetProperty("Nodes");
                                if (nodesProp != null)
                                {
                                    object nodes = nodesProp.GetValue(program);
                                    if (nodes != null)
                                    {
                                        // ノードの数を確認
                                        var nodesCollection = nodes as System.Collections.ICollection;
                                        if (nodesCollection != null && nodesCollection.Count > 0)
                                        {
                                            yarnProjectValid = true;
                                            UnityEngine.Debug.Log($"DebugSceneBuilder: YarnProject is valid with {nodesCollection.Count} node(s).");
                                            
                                            // Startノードが存在するか確認
                                            MethodInfo containsNodeMethod = programType.GetMethod("Contains", new[] { typeof(string) });
                                            if (containsNodeMethod != null)
                                            {
                                                bool hasStartNode = (bool)containsNodeMethod.Invoke(program, new object[] { "Start" });
                                                if (!hasStartNode)
                                                {
                                                    UnityEngine.Debug.LogWarning("DebugSceneBuilder: YarnProject does not contain 'Start' node. Please check your Yarn scripts.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // ノードが存在しない場合、YarnProjectアセットを再インポートして再試行
                                            UnityEngine.Debug.LogWarning("DebugSceneBuilder: YarnProject has no nodes. Attempting to reimport the YarnProject asset...");
                                            AssetDatabase.ImportAsset(yarnProjectPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                                            
                                            // 再インポート後、再度YarnProjectを取得して検証
                                            yarnProject = AssetDatabase.LoadAssetAtPath<YarnProject>(yarnProjectPath);
                                            if (yarnProject != null)
                                            {
                                                program = programProp.GetValue(yarnProject);
                                                if (program != null)
                                                {
                                                    nodes = nodesProp.GetValue(program);
                                                    if (nodes != null)
                                                    {
                                                        nodesCollection = nodes as System.Collections.ICollection;
                                                        if (nodesCollection != null && nodesCollection.Count > 0)
                                                        {
                                                            yarnProjectValid = true;
                                                            UnityEngine.Debug.Log($"DebugSceneBuilder: YarnProject is now valid with {nodesCollection.Count} node(s) after reimport.");
                                                            
                                                            // DialogueRunnerの参照を更新
                                                            yarnProjectProp.objectReferenceValue = yarnProject;
                                                            soRunner.ApplyModifiedProperties();
                                                        }
                                                        else
                                                        {
                                                            UnityEngine.Debug.LogError("DebugSceneBuilder: YarnProject still has no nodes after reimport. Please check that Yarn files exist in Assets/Resources/Yarn/ and that the Project.yarnproject file is correctly configured.");
                                                        }
                                                    }
                                                }
                                            }
                                            
                                            if (!yarnProjectValid)
                                            {
                                                UnityEngine.Debug.LogError("DebugSceneBuilder: Failed to import YarnProject. Please manually select the Project.yarnproject asset in the Inspector and click 'Reimport' or 'Compile'.");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Programがnullの場合、YarnProjectアセットを再インポートして再試行
                                UnityEngine.Debug.LogWarning("DebugSceneBuilder: YarnProject Program is null. Attempting to reimport the YarnProject asset...");
                                AssetDatabase.ImportAsset(yarnProjectPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                                
                                // 再インポート後、再度YarnProjectを取得して検証
                                yarnProject = AssetDatabase.LoadAssetAtPath<YarnProject>(yarnProjectPath);
                                if (yarnProject != null)
                                {
                                    program = programProp.GetValue(yarnProject);
                                    if (program != null)
                                    {
                                        Type programType = program.GetType();
                                        PropertyInfo nodesProp = programType.GetProperty("Nodes");
                                        if (nodesProp != null)
                                        {
                                            object nodes = nodesProp.GetValue(program);
                                            if (nodes != null)
                                            {
                                                var nodesCollection = nodes as System.Collections.ICollection;
                                                if (nodesCollection != null && nodesCollection.Count > 0)
                                                {
                                                    yarnProjectValid = true;
                                                    UnityEngine.Debug.Log($"DebugSceneBuilder: YarnProject is now valid with {nodesCollection.Count} node(s) after reimport.");
                                                    
                                                    // DialogueRunnerの参照を更新
                                                    yarnProjectProp.objectReferenceValue = yarnProject;
                                                    soRunner.ApplyModifiedProperties();
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                if (!yarnProjectValid)
                                {
                                    UnityEngine.Debug.LogError("DebugSceneBuilder: YarnProject Program is still null after reimport. The YarnProject asset may be corrupted. Please try recreating it or check Yarn Spinner package installation.");
                                }
                            }
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning("DebugSceneBuilder: Could not verify YarnProject compilation status. Assuming it's valid.");
                            yarnProjectValid = true; // 検証できない場合は有効とみなす
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogWarning($"DebugSceneBuilder: Could not verify YarnProject compilation status due to reflection error: {ex.Message}. Attempting to reimport...");
                        
                        // エラーが発生した場合も再インポートを試みる
                        try
                        {
                            AssetDatabase.ImportAsset(yarnProjectPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                            yarnProject = AssetDatabase.LoadAssetAtPath<YarnProject>(yarnProjectPath);
                            if (yarnProject != null)
                            {
                                yarnProjectProp.objectReferenceValue = yarnProject;
                                soRunner.ApplyModifiedProperties();
                                UnityEngine.Debug.Log("DebugSceneBuilder: YarnProject reimported. Please verify manually if it's working correctly.");
                                yarnProjectValid = true; // 再インポート後は有効とみなす
                            }
                        }
                        catch (Exception reimportEx)
                        {
                            UnityEngine.Debug.LogError($"DebugSceneBuilder: Failed to reimport YarnProject: {reimportEx.Message}");
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("DialogueRunner: 'yarnProject' property not found. Check Yarn Spinner version.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"YarnProject not found at {yarnProjectPath}. Please ensure the file exists.");
                
                // Yarnファイルが存在するか確認
                string[] yarnFiles = System.IO.Directory.GetFiles("Assets/Resources/Yarn", "*.yarn", System.IO.SearchOption.TopDirectoryOnly);
                if (yarnFiles.Length > 0)
                {
                    UnityEngine.Debug.LogWarning($"DebugSceneBuilder: Found {yarnFiles.Length} Yarn file(s) in Assets/Resources/Yarn/, but YarnProject asset is missing or invalid.");
                    UnityEngine.Debug.LogWarning("DebugSceneBuilder: Please create a new YarnProject asset:");
                    UnityEngine.Debug.LogWarning("  1. Right-click in Assets/Resources/Yarn/ folder");
                    UnityEngine.Debug.LogWarning("  2. Select 'Create > Yarn Spinner > Yarn Project'");
                    UnityEngine.Debug.LogWarning("  3. Name it 'Project'");
                    UnityEngine.Debug.LogWarning("  4. The YarnProject should automatically detect .yarn files in the same directory");
                }
                else
                {
                    UnityEngine.Debug.LogError("DebugSceneBuilder: No Yarn files found in Assets/Resources/Yarn/. Please ensure DebugScript.yarn exists.");
                }
            }
            
            SerializedProperty variableStorageProp = soRunner.FindProperty("variableStorage");
            if (variableStorageProp != null)
            {
                variableStorageProp.objectReferenceValue = managerObj.GetComponent<InMemoryVariableStorage>();
                soRunner.ApplyModifiedProperties();
            }
            else
            {
                UnityEngine.Debug.LogError("DialogueRunner: 'variableStorage' property not found. Check Yarn Spinner version.");
            }
            
            SerializedProperty startNodeProp = soRunner.FindProperty("startNode");
            if (startNodeProp != null)
            {
                startNodeProp.stringValue = "Start";
                soRunner.ApplyModifiedProperties();
                UnityEngine.Debug.Log("DebugSceneBuilder: Successfully set DialogueRunner start node to 'Start'.");
            }
            else
            {
                UnityEngine.Debug.LogWarning("DialogueRunner: 'startNode' property not found.");
            }
            
            // Yarn Spinnerのバージョンによってプロパティ名が異なる可能性があるため、両方を試す
            // YarnProjectが有効な場合のみautoStartをtrueに設定
            SerializedProperty startAutomaticallyProp = soRunner.FindProperty("autoStart");
            if (startAutomaticallyProp == null)
            {
                // 代替プロパティ名を試す
                startAutomaticallyProp = soRunner.FindProperty("startAutomatically");
            }
            
            if (startAutomaticallyProp != null)
            {
                // YarnProjectが有効な場合のみautoStartをtrueに設定
                // 無効な場合はfalseに設定し、手動で開始する必要がある
                startAutomaticallyProp.boolValue = yarnProjectValid;
                soRunner.ApplyModifiedProperties();
                
                if (yarnProjectValid)
                {
                    UnityEngine.Debug.Log("DebugSceneBuilder: Successfully set DialogueRunner auto-start property to true (YarnProject is valid).");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("DebugSceneBuilder: Set DialogueRunner auto-start property to false because YarnProject is not valid. Please compile the YarnProject asset and manually start the dialogue.");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("DialogueRunner: Neither 'autoStart' nor 'startAutomatically' property found. Dialogue will not start automatically. You may need to call StartDialogue() manually.");
            }
            
            soRunner.ApplyModifiedProperties();

            // Configure ScenarioManager
            SerializedObject soScenario = new SerializedObject(scenarioManager);
            soScenario.FindProperty("m_DialogueRunner").objectReferenceValue = dialogueRunner;
            soScenario.FindProperty("m_ChatController").objectReferenceValue = chatController;
            soScenario.ApplyModifiedProperties();

            UnityEngine.Debug.Log("Debug Scene Setup Complete. Please save the scene as 'Assets/Scenes/DebugChatScene.unity'.");
        }

        public static void BuildAndSave()
        {
            SetupScene();
            // Create Scenes folder if not exists
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/Scenes/DebugChatScene.unity");
            UnityEngine.Debug.Log("DebugChatScene saved to Assets/Scenes/DebugChatScene.unity");
        }

        private static void StretchToFill(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }
    }
}
