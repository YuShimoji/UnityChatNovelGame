using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.EditorTools
{
    public static class FragmentListUISetup
    {
        [MenuItem("Project FoundPhone/Setup/Fragment List UI")]
        public static void Setup()
        {
            var existing = Object.FindFirstObjectByType<FragmentListUI>();
            if (existing != null)
            {
                Debug.Log("[FragmentListUISetup] FragmentListUI already exists in scene.");
                return;
            }

            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[FragmentListUISetup] No Canvas found in scene. Add a Canvas first.");
                return;
            }

            var go = new GameObject("FragmentListUI");
            go.transform.SetParent(canvas.transform, false);
            Undo.RegisterCreatedObjectUndo(go, "Create FragmentListUI");
            go.AddComponent<FragmentListUI>();

            Debug.Log("[FragmentListUISetup] Created FragmentListUI in scene. Save the scene to persist.");
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Project FoundPhone/Validate/Fragment List UI")]
        public static void Validate()
        {
            var fragmentUI = Object.FindFirstObjectByType<FragmentListUI>();
            var deductionBoard = Object.FindFirstObjectByType<DeductionBoard>();

            int issues = 0;

            if (fragmentUI == null)
            {
                Debug.LogError("[Validate] FragmentListUI not found in scene. Run Setup first.");
                issues++;
            }

            if (deductionBoard == null)
            {
                Debug.LogWarning("[Validate] DeductionBoard not found in scene. Fragment list will show empty.");
                issues++;
            }

            if (issues == 0)
            {
                Debug.Log("[Validate] Fragment List UI wiring OK.");
            }
            else
            {
                Debug.LogWarning($"[Validate] {issues} issue(s) found.");
            }
        }
    }
}
