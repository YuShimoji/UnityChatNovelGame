using System;
using ProjectFoundPhone.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    [InitializeOnLoad]
    internal static class NarrativeGenPreviewMenu
    {
        private const string MenuPath = "Tools/FoundPhone/Play NarrativeGen JSON...";
        private const string PreviousSceneKey = "FoundPhone.NarrativeGenPreview.PreviousScene";
        private const string PreviewPendingKey = "FoundPhone.NarrativeGenPreview.Pending";

        static NarrativeGenPreviewMenu()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem(MenuPath, true)]
        private static bool CanPlay() => !EditorApplication.isPlayingOrWillChangePlaymode;

        [MenuItem(MenuPath)]
        private static void Play()
        {
            string file = EditorUtility.OpenFilePanel("Play NarrativeGen JSON", string.Empty, "json");
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            try
            {
                NarrativeGenPlayback.ValidateModelFile(file);
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(NarrativeGenPreviewRequest.ScenePath);
                if (scene == null)
                {
                    throw new InvalidOperationException("The FoundPhone chat scene could not be opened.");
                }
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return;
                }

                var previous = EditorSceneManager.playModeStartScene;
                SessionState.SetString(PreviousSceneKey,
                    previous == null ? string.Empty : AssetDatabase.GetAssetPath(previous));
                SessionState.SetBool(PreviewPendingKey, true);
                NarrativeGenPreviewRequest.Queue(file);
                EditorSceneManager.playModeStartScene = scene;
                EditorApplication.EnterPlaymode();
            }
            catch (Exception error)
            {
                RestoreStartScene();
                Debug.LogException(error);
                EditorUtility.DisplayDialog("NarrativeGen JSON", error.Message, "OK");
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                RestoreStartScene();
            }
        }

        private static void RestoreStartScene()
        {
            if (!SessionState.GetBool(PreviewPendingKey, false))
            {
                return;
            }

            string previous = SessionState.GetString(PreviousSceneKey, string.Empty);
            EditorSceneManager.playModeStartScene = string.IsNullOrEmpty(previous)
                ? null
                : AssetDatabase.LoadAssetAtPath<SceneAsset>(previous);
            NarrativeGenPreviewRequest.Clear();
            SessionState.EraseString(PreviousSceneKey);
            SessionState.EraseBool(PreviewPendingKey);
        }
    }
}
