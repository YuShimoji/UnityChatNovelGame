#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.MVP.Editor
{
    /// <summary>
    /// MVP検証用の一時的テストヘルパー。
    /// メニューアイテムからPlay Mode中にボタンクリックをシミュレートする。
    /// 検証完了後に削除すること。
    /// </summary>
    public static class MVPTestHelper
    {
        [MenuItem("Tools/FoundPhone/Test Click Start")]
        public static void ClickStart() => ClickButton("StartButton");

        [MenuItem("Tools/FoundPhone/Test Click ChoiceA")]
        public static void ClickChoiceA() => ClickButton("ChoiceA");

        [MenuItem("Tools/FoundPhone/Test Click ChoiceB")]
        public static void ClickChoiceB() => ClickButton("ChoiceB");

        [MenuItem("Tools/FoundPhone/Test Click Restart")]
        public static void ClickRestart() => ClickButton("RestartButton");

        [MenuItem("Tools/FoundPhone/Test Rapid Start x5")]
        public static void RapidStart()
        {
            for (int i = 0; i < 5; i++) ClickButton("StartButton");
        }

        [MenuItem("Tools/FoundPhone/Test Rapid ChoiceA x5")]
        public static void RapidChoiceA()
        {
            for (int i = 0; i < 5; i++) ClickButton("ChoiceA");
        }

        [MenuItem("Tools/FoundPhone/Test Rapid ChoiceB x5")]
        public static void RapidChoiceB()
        {
            for (int i = 0; i < 5; i++) ClickButton("ChoiceB");
        }

        [MenuItem("Tools/FoundPhone/Test Synthesis (Debug + Phone)")]
        public static void TestSynthesis()
        {
             if (!Application.isPlaying)
            {
                Debug.LogWarning("[MVPTestHelper] Play Modeでのみ使用可能です。");
                return;
            }

            var board = UnityEngine.Object.FindObjectOfType<DeductionBoard>();
            if (board == null)
            {
                 Debug.LogError("[MVPTestHelper] DeductionBoard not found.");
                 return;
            }

            // Load topics
            var topicA = Resources.Load<TopicData>("Topics/debug_topic_01");
            var topicB = Resources.Load<TopicData>("Topics/topic_found_phone");

            if (topicA == null || topicB == null)
            {
                Debug.LogError($"[MVPTestHelper] Failed to load topics. A: {topicA}, B: {topicB}");
                return;
            }

            Debug.Log($"[MVPTestHelper] Attempting synthesis: {topicA.name} + {topicB.name}");
            // Add topics first just in case they aren't there (to enable testing synthesis logic in isolation)
            board.AddTopic(topicA);
            board.AddTopic(topicB);

            // CheckSynthesis is private, use Reflection
            var method = typeof(DeductionBoard).GetMethod("CheckSynthesis", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                bool success = (bool)method.Invoke(board, new object[] { topicA, topicB });
                Debug.Log($"[MVPTestHelper] Synthesis Result: {success}");
            }
            else
            {
                 Debug.LogError("[MVPTestHelper] CheckSynthesis method not found via Reflection.");
            }
        }

        private static void ClickButton(string buttonName)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[MVPTestHelper] Play Modeでのみ使用可能です。");
                return;
            }
            // FindObjectsOfTypeAllは非アクティブも含めて検索可能
            var buttons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var btn in buttons)
            {
                // インスタンス化されたボタンは "ChoiceA(Clone)" のような名前になることがあるが、
                // ChatControllerの修正で "ChoiceA" 等に命名されているはず。
                // 念のため部分一致も考慮するか、完全一致でいくか。
                // 今回は完全一致でいく（ChatController修正済みのため）
                if (btn.gameObject.name == buttonName && btn.gameObject.activeInHierarchy)
                {
                    btn.onClick.Invoke();
                    Debug.Log("[MVPTestHelper] Clicked: " + buttonName);
                    return;
                }
            }
            Debug.LogWarning("[MVPTestHelper] Button not found or inactive: " + buttonName);
        }
    }
}
#endif
