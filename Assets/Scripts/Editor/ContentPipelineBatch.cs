#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// batchmode / CI 向け。例（-batchmode -quit と併用）:
    /// -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunYarnValidatorBatch
    /// -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunSyncAuthoringAssetsBatch
    /// -executeMethod ProjectFoundPhone.Editor.ContentPipelineBatch.RunValidateThenSyncBatch
    /// </summary>
    public static class ContentPipelineBatch
    {
        /// <summary>
        /// -executeMethod から直接呼ぶ（-quit 併用時は delayCall だと処理前に終了することがあるため同期実行）。
        /// </summary>
        public static void RunYarnValidatorBatch()
        {
            ExecuteYarnValidatorBatch();
        }

        public static void RunSyncAuthoringAssetsBatch()
        {
            ExecuteSyncBatch();
        }

        public static void RunValidateThenSyncBatch()
        {
            ExecuteValidateThenSyncBatch();
        }

        private static void ExecuteYarnValidatorBatch()
        {
            try
            {
                int errors = YarnContentValidator.ValidateAllYarnFilesLogToConsole();
                EditorApplication.Exit(errors > 0 ? 1 : 0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ContentPipelineBatch: Validator failed. {ex}");
                EditorApplication.Exit(1);
            }
        }

        private static void ExecuteSyncBatch()
        {
            try
            {
                int changedCount = YarnSOGenerator.SyncAllAuthoringAssets();
                Debug.Log($"ContentPipelineBatch: SyncAuthoringAssets OK (changed={changedCount}).");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ContentPipelineBatch: Sync failed. {ex}");
                EditorApplication.Exit(1);
            }
        }

        private static void ExecuteValidateThenSyncBatch()
        {
            try
            {
                int errors = YarnContentValidator.ValidateAllYarnFilesLogToConsole();
                if (errors > 0)
                {
                    Debug.LogError("ContentPipelineBatch: Stopping before Sync due to validator errors.");
                    EditorApplication.Exit(1);
                    return;
                }

                int changedCount = YarnSOGenerator.SyncAllAuthoringAssets();
                Debug.Log($"ContentPipelineBatch: Validate+Sync OK (changed={changedCount}).");
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ContentPipelineBatch: ValidateThenSync failed. {ex}");
                EditorApplication.Exit(1);
            }
        }
    }
}
#endif
