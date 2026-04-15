using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.EditorTools
{
    public static class ContradictionSystemSetup
    {
        [MenuItem("Tools/FoundPhone/Setup/Contradiction System")]
        public static void Setup()
        {
            bool managerReady = EnsureContradictionManager();
            bool feedbackReady = EnsureContradictionFeedbackController();

            if (managerReady && feedbackReady)
            {
                Debug.Log("[ContradictionSystemSetup] Setup complete. Mark scene as dirty.");
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
        }

        [MenuItem("Tools/FoundPhone/Verification/Contradiction System Wiring")]
        public static void Validate()
        {
            var manager = Object.FindFirstObjectByType<ContradictionManager>();
            var feedback = Object.FindFirstObjectByType<ContradictionFeedbackController>();
            var chat = Object.FindFirstObjectByType<ChatController>();

            int issues = 0;

            if (manager == null)
            {
                Debug.LogError("[Validate] ContradictionManager not found in scene. Run Setup first.");
                issues++;
            }
            else
            {
                var so = new SerializedObject(manager);
                if (so.FindProperty("m_Database").objectReferenceValue == null)
                {
                    Debug.LogError("[Validate] ContradictionManager.m_Database is null.");
                    issues++;
                }
            }

            if (feedback == null)
            {
                Debug.LogError("[Validate] ContradictionFeedbackController not found in scene. Run Setup first.");
                issues++;
            }
            else
            {
                var so = new SerializedObject(feedback);
                if (so.FindProperty("m_ChatController").objectReferenceValue == null)
                {
                    Debug.LogError("[Validate] ContradictionFeedbackController.m_ChatController is null.");
                    issues++;
                }
            }

            if (chat == null)
            {
                Debug.LogWarning("[Validate] ChatController not found in scene.");
                issues++;
            }

            if (issues == 0)
            {
                Debug.Log("[Validate] Contradiction system wiring OK.");
            }
            else
            {
                Debug.LogWarning($"[Validate] {issues} issue(s) found.");
            }
        }

        private static bool EnsureContradictionManager()
        {
            var existing = Object.FindFirstObjectByType<ContradictionManager>();
            if (existing != null)
            {
                Debug.Log("[ContradictionSystemSetup] ContradictionManager already exists in scene.");
                return WireContradictionManager(existing);
            }

            var go = new GameObject("ContradictionManager");
            Undo.RegisterCreatedObjectUndo(go, "Create ContradictionManager");
            var manager = go.AddComponent<ContradictionManager>();

            Debug.Log("[ContradictionSystemSetup] Created ContradictionManager GameObject.");
            return WireContradictionManager(manager);
        }

        private static bool WireContradictionManager(ContradictionManager manager)
        {
            var so = new SerializedObject(manager);

            // Wire m_Database
            if (so.FindProperty("m_Database").objectReferenceValue == null)
            {
                var db = AssetDatabase.LoadAssetAtPath<ContradictionDatabase>(
                    "Assets/Resources/Contradictions/ContradictionDatabase.asset");
                if (db != null)
                {
                    so.FindProperty("m_Database").objectReferenceValue = db;
                    Debug.Log("[ContradictionSystemSetup] Assigned ContradictionDatabase to manager.");
                }
                else
                {
                    Debug.LogError("[ContradictionSystemSetup] ContradictionDatabase.asset not found at expected path.");
                    return false;
                }
            }

            so.ApplyModifiedProperties();
            return true;
        }

        private static bool EnsureContradictionFeedbackController()
        {
            var existing = Object.FindFirstObjectByType<ContradictionFeedbackController>();
            if (existing != null)
            {
                Debug.Log("[ContradictionSystemSetup] ContradictionFeedbackController already exists in scene.");
                return WireContradictionFeedbackController(existing);
            }

            // Find Canvas to parent under
            var canvas = Object.FindFirstObjectByType<Canvas>();
            Transform parent = canvas != null ? canvas.transform : null;

            var go = new GameObject("ContradictionFeedback");
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }
            Undo.RegisterCreatedObjectUndo(go, "Create ContradictionFeedbackController");

            var feedback = go.AddComponent<ContradictionFeedbackController>();
            Debug.Log("[ContradictionSystemSetup] Created ContradictionFeedbackController GameObject.");

            return WireContradictionFeedbackController(feedback);
        }

        private static bool WireContradictionFeedbackController(ContradictionFeedbackController feedback)
        {
            var so = new SerializedObject(feedback);

            if (so.FindProperty("m_ChatController").objectReferenceValue == null)
            {
                var chat = Object.FindFirstObjectByType<ChatController>();
                if (chat != null)
                {
                    so.FindProperty("m_ChatController").objectReferenceValue = chat;
                    Debug.Log("[ContradictionSystemSetup] Assigned ChatController to feedback controller.");
                }
                else
                {
                    Debug.LogWarning("[ContradictionSystemSetup] ChatController not found. Assign manually after opening a chat scene.");
                }
            }

            so.ApplyModifiedProperties();
            return true;
        }
    }
}
