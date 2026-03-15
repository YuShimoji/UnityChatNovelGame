using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// サブスレッド切替ボタンを管理するコントローラ。
    /// ScenarioManager.OnThreadDeclared を購読し、ボタンを動的表示する。
    /// 最小スライス: メイン⇔サブスレッド1本のトグル切替のみ。
    /// </summary>
    public class ThreadSwitcherController : MonoBehaviour
    {
        [SerializeField] private ChatController m_ChatController;

        private ScenarioManager m_ScenarioManager;
        private GameObject m_ButtonObj;
        private TextMeshProUGUI m_ButtonLabel;
        private string m_CurrentThreadId;
        private string m_CurrentThreadName;

        private void Start()
        {
            m_ScenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (m_ChatController == null)
            {
                m_ChatController = FindFirstObjectByType<ChatController>();
            }

            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.OnThreadDeclared += OnThreadDeclared;
            }

            // 起動時はボタン非表示
            CreateButton();
            m_ButtonObj.SetActive(false);
        }

        private void OnDestroy()
        {
            if (m_ScenarioManager != null)
            {
                m_ScenarioManager.OnThreadDeclared -= OnThreadDeclared;
            }
        }

        private void CreateButton()
        {
            // ScrollRect の上部にオーバーレイボタンを配置
            // ChatController と同じ Canvas を使用
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
            }
            if (canvas == null)
            {
                Debug.LogWarning("ThreadSwitcherController: Canvas not found.");
                return;
            }

            m_ButtonObj = new GameObject("ThreadSwitchButton");
            m_ButtonObj.transform.SetParent(canvas.transform, false);

            // RectTransform: 右上に配置
            var rt = m_ButtonObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-16f, -80f);
            rt.sizeDelta = new Vector2(160f, 40f);

            // 背景
            var bg = m_ButtonObj.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.25f, 0.35f, 0.95f);

            // ボタンコンポーネント
            var btn = m_ButtonObj.AddComponent<Button>();
            btn.onClick.AddListener(OnToggleThread);

            // ラベル
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(m_ButtonObj.transform, false);
            var labelRt = labelObj.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = new Vector2(8f, 4f);
            labelRt.offsetMax = new Vector2(-8f, -4f);

            m_ButtonLabel = labelObj.AddComponent<TextMeshProUGUI>();
            m_ButtonLabel.fontSize = 14f;
            m_ButtonLabel.alignment = TextAlignmentOptions.Center;
            m_ButtonLabel.color = Color.white;
            m_ButtonLabel.text = "";
        }

        private void OnThreadDeclared(string threadId, string displayName)
        {
            // 最小スライス: 最初に宣言されたスレッドのみ対応
            if (m_CurrentThreadId != null) return;

            m_CurrentThreadId = threadId;
            m_CurrentThreadName = displayName;

            m_ButtonObj.SetActive(true);
            UpdateLabel();
        }

        private void OnToggleThread()
        {
            if (m_ChatController == null || m_CurrentThreadId == null) return;

            bool isOnMain = m_ChatController.ActiveThreadId == null;

            if (isOnMain)
            {
                // メイン → サブスレッドへ切替
                var thread = m_ScenarioManager?.GetDeclaredThread(m_CurrentThreadId);
                m_ChatController.SwitchToThread(m_CurrentThreadId, thread?.ChatHistory);
            }
            else
            {
                // サブスレッド → メインへ切替
                m_ChatController.SwitchToThread(null);
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (m_ButtonLabel == null) return;

            bool isOnMain = m_ChatController == null || m_ChatController.ActiveThreadId == null;
            m_ButtonLabel.text = isOnMain ? m_CurrentThreadName : "Main";
        }

        /// <summary>スレッド切替状態をリセット（シナリオ再開始時用）</summary>
        public void Reset()
        {
            m_CurrentThreadId = null;
            m_CurrentThreadName = null;
            if (m_ButtonObj != null)
            {
                m_ButtonObj.SetActive(false);
            }
        }
    }
}
