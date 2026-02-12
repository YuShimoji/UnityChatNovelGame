using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

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

        private static void ClickButton(string buttonName)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[MVPTestHelper] Play Modeでのみ使用可能です。");
                return;
            }
            var buttons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var btn in buttons)
            {
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
