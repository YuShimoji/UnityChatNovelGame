using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 繝√Ε繝・ヨ逕ｻ髱｢縺ｮUI蛻ｶ蠕｡繧定｡後≧繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ
    /// ScrollRect + VerticalLayoutGroup + ContentSizeFitter繧剃ｽｿ逕ｨ縺励◆繝｡繝・そ繝ｼ繧ｸ陦ｨ遉ｺ繧ｷ繧ｹ繝・Β
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ChatController : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private VerticalLayoutGroup m_LayoutGroup;
        [SerializeField] private GameObject m_MessageBubblePrefab;
        [SerializeField] private GameObject m_TypingIndicator;
        [SerializeField] private TMP_InputField m_InputField;
        [SerializeField] private Button m_SendButton;
        [SerializeField] private float m_AutoScrollThreshold = 0.1f; // 閾ｪ蜍輔せ繧ｯ繝ｭ繝ｼ繝ｫ繧貞ｮ溯｡後☆繧矩明蛟､・・.0-1.0・・
        [Header("Choice Settings")]
        [SerializeField] private GameObject m_ChoiceButtonPrefab;
        [SerializeField] private Transform m_ChoiceContainer;


        private bool m_IsUserScrolling = false;
        private float m_LastScrollPosition = 1.0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            if (m_SendButton != null)
            {
                m_SendButton.onClick.AddListener(OnSubmit);
            }

            if (m_InputField != null)
            {
                // Enter繧ｭ繝ｼ縺ｧ繧る∽ｿ｡縺ｧ縺阪ｋ繧医≧縺ｫ縺吶ｋ (Optional)
                m_InputField.onSubmit.AddListener((text) => OnSubmit());
            }
        }

        private void Update()
        {
            // TODO: 繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ菴咲ｽｮ縺ｮ逶｣隕悶→繝ｦ繝ｼ繧ｶ繝ｼ謫堺ｽ懊・讀懃衍
            CheckUserScrollInput();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 蠢・ｦ√↑繧ｳ繝ｳ繝昴・繝阪Φ繝医・蛻晄悄蛹・        /// </summary>
        private void InitializeComponents()
        {
            if (m_ScrollRect == null)
            {
                m_ScrollRect = GetComponent<ScrollRect>();
            }

            if (m_LayoutGroup == null && m_ScrollRect != null && m_ScrollRect.content != null)
            {
                m_LayoutGroup = m_ScrollRect.content.GetComponent<VerticalLayoutGroup>();
            }

            // m_MessageBubblePrefab縲［_TypingIndicator縺ｮnull繝√ぉ繝・け縺ｨ隴ｦ蜻・            if (m_MessageBubblePrefab == null)
            {
                Debug.LogWarning("ChatController: m_MessageBubblePrefab is not assigned. Message bubbles cannot be created.");
            }

            if (m_TypingIndicator == null)
            {
                Debug.LogWarning("ChatController: m_TypingIndicator is not assigned. Typing indicator will not be displayed.");
            }
        }

        /// <summary>
        /// 繝ｦ繝ｼ繧ｶ繝ｼ縺梧焔蜍輔〒繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ縺励※縺・ｋ縺九ｒ讀懃衍
        /// </summary>
        private void CheckUserScrollInput()
        {
            if (m_ScrollRect == null)
            {
                return;
            }

            float currentScrollPosition = m_ScrollRect.verticalNormalizedPosition;

            // 繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ菴咲ｽｮ縺御ｸ九°繧我ｸ螳壻ｻ･荳企屬繧後※縺・ｋ蝣ｴ蜷医√Θ繝ｼ繧ｶ繝ｼ縺碁℃蜴ｻ繝ｭ繧ｰ繧定ｦ九※縺・ｋ縺ｨ蛻､螳・            if (currentScrollPosition < (1.0f - m_AutoScrollThreshold))
            {
                m_IsUserScrolling = true;
            }
            // 繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ菴咲ｽｮ縺・.0縺ｫ霑代＞蝣ｴ蜷医√Θ繝ｼ繧ｶ繝ｼ縺ｯ譛譁ｰ繝｡繝・そ繝ｼ繧ｸ繧定ｦ九※縺・ｋ
            else if (currentScrollPosition >= 0.99f)
            {
                m_IsUserScrolling = false;
            }

            m_LastScrollPosition = currentScrollPosition;
        }

        /// <summary>
        /// 繝｡繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ縺ｮPrefab繧偵う繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ蛹・        /// </summary>
        /// <param name="charID">繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼID・郁・蛻・逶ｸ謇九・蛻､螳壹↓菴ｿ逕ｨ・・/param>
        /// <param name="text">繝｡繝・そ繝ｼ繧ｸ繝・く繧ｹ繝・/param>
        /// <returns>逕滓・縺輔ｌ縺檬ameObject</returns>
        private GameObject CreateMessageBubble(string charID, string text)
        {
            if (m_MessageBubblePrefab == null)
            {
                Debug.LogError("ChatController: Cannot create message bubble. Prefab is not assigned.");
                return null;
            }

            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                Debug.LogError("ChatController: Cannot create message bubble. ScrollRect or content is not assigned.");
                return null;
            }

            // Prefab縺九ｉ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ繧堤函謌・            GameObject messageBubble = Instantiate(m_MessageBubblePrefab, m_ScrollRect.content);

            // charID縺ｫ蠢懊§縺ｦ蜿ｳ蟇・○/蟾ｦ蟇・○繧定ｨｭ螳夲ｼ・player"縺ｮ蝣ｴ蜷医・蜿ｳ蟇・○縲√◎繧御ｻ･螟悶・蟾ｦ蟇・○・・            RectTransform rectTransform = messageBubble.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (charID == "player")
                {
                    // 蜿ｳ蟇・○: Anchor繧貞承蛛ｴ縺ｫ險ｭ螳・                    rectTransform.anchorMin = new Vector2(1.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    rectTransform.pivot = new Vector2(1.0f, 1.0f);
                }
                else
                {
                    // 蟾ｦ蟇・○: Anchor繧貞ｷｦ蛛ｴ縺ｫ險ｭ螳・                    rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
                    rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    rectTransform.pivot = new Vector2(0.0f, 1.0f);
                }
            }

            // TextMeshPro繧ｳ繝ｳ繝昴・繝阪Φ繝医↓text繧定ｨｭ螳・            TextMeshProUGUI textComponent = messageBubble.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            }
            else
            {
                Debug.LogWarning("ChatController: TextMeshProUGUI component not found in message bubble prefab.");
            }

            // ContentSizeFitter縺ｧ鬮倥＆繧定・蜍戊ｪｿ謨ｴ
            ContentSizeFitter sizeFitter = messageBubble.GetComponent<ContentSizeFitter>();
            if (sizeFitter != null)
            {
                sizeFitter.SetLayoutVertical();
            }

            return messageBubble;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 譁ｰ縺励＞繝｡繝・そ繝ｼ繧ｸ繧偵メ繝｣繝・ヨ縺ｫ霑ｽ蜉
        /// </summary>
        /// <param name="charID">繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼID・井ｾ・ "player", "npc_001"・・/param>
        /// <param name="text">繝｡繝・そ繝ｼ繧ｸ繝・く繧ｹ繝・/param>
        public void AddMessage(string charID, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("ChatController: Attempted to add empty message.");
                return;
            }

            // 繝｡繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ縺ｮ逕滓・縺ｨ霑ｽ蜉

            // CreateMessageBubble()縺ｧ繝｡繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ繧堤函謌撰ｼ域里縺ｫcontent縺ｮ蟄舌→縺励※霑ｽ蜉貂医∩・・            GameObject messageBubble = CreateMessageBubble(charID, text);
            if (messageBubble == null)
            {
                return;
            }

            // 繝ｦ繝ｼ繧ｶ繝ｼ縺碁℃蜴ｻ繝ｭ繧ｰ繧定ｦ九※縺・↑縺・ｴ蜷医・縺ｿAutoScroll()繧貞ｮ溯｡・            if (!m_IsUserScrolling)
            {
                AutoScroll();
            }
        }

        /// <summary>
        /// 繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ縺ｮ陦ｨ遉ｺ/髱櫁｡ｨ遉ｺ繧貞・繧頑崛縺・        /// </summary>
        /// <param name="show">陦ｨ遉ｺ縺吶ｋ蝣ｴ蜷・rue</param>
        public void ShowTypingIndicator(bool show)
        {
            if (m_TypingIndicator != null)
            {
                m_TypingIndicator.SetActive(show);
                
                if (show)
                {
                    // 蟶ｸ縺ｫ譛蠕悟ｰｾ縺ｫ陦ｨ遉ｺ
                    m_TypingIndicator.transform.SetAsLastSibling();
                    AutoScroll();
                }
            }
        }



        /// <summary>
        /// 驕ｸ謚櫁い繧定｡ｨ遉ｺ縺吶ｋ
        /// </summary>
        /// <param name="options">驕ｸ謚櫁い縺ｮ繝・く繧ｹ繝医Μ繧ｹ繝・/param>
        /// <param name="onSelected">驕ｸ謚樊凾縺ｮ繧ｳ繝ｼ繝ｫ繝舌ャ繧ｯ (index)</param>
        public void ShowChoices(List<string> options, System.Action<int> onSelected)
        {
            if (m_ChoiceButtonPrefab == null || m_ChoiceContainer == null)
            {
                Debug.LogError("ChatController: ChoiceButtonPrefab or ChoiceContainer is not assigned.");
                return;
            }

            // 譌｢蟄倥・驕ｸ謚櫁い繧偵け繝ｪ繧｢
            HideChoices();

            m_ChoiceContainer.gameObject.SetActive(true);

            // 蜈･蜉帶ｬ・ｒ髱櫁｡ｨ遉ｺ縺ｫ縺吶ｋ・医が繝励す繝ｧ繝ｳ・・            if (m_InputField != null) m_InputField.gameObject.SetActive(false);
            if (m_SendButton != null) m_SendButton.gameObject.SetActive(false);

            for (int i = 0; i < options.Count; i++)
            {
                int index = i; // 繧ｭ繝｣繝励メ繝｣逕ｨ
                GameObject buttonObj = Instantiate(m_ChoiceButtonPrefab, m_ChoiceContainer);
                
                // 繝懊ち繝ｳ縺ｮ繝・く繧ｹ繝郁ｨｭ螳・                TextMeshProUGUI btnText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = options[i];
                }

                // 繧ｯ繝ｪ繝・け繧､繝吶Φ繝郁ｨｭ螳・                Button btn = buttonObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        HideChoices();
                        onSelected?.Invoke(index);
                    });
                }
            }

            // 驕ｸ謚櫁い縺瑚｡ｨ遉ｺ縺輔ｌ縺溘ｉ譛荳矩Κ縺ｸ繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ
            AutoScroll();
        }

        /// <summary>
        /// 驕ｸ謚櫁い繧帝撼陦ｨ遉ｺ縺ｫ縺吶ｋ
        /// </summary>
        public void HideChoices()
        {
            if (m_ChoiceContainer != null)
            {
                // 蟄占ｦ∫ｴ繧貞・縺ｦ蜑企勁
                foreach (Transform child in m_ChoiceContainer)
                {
                    Destroy(child.gameObject);
                }
                m_ChoiceContainer.gameObject.SetActive(false);
            }

            // 蜈･蜉帶ｬ・ｒ蜀崎｡ｨ遉ｺ
            if (m_InputField != null) m_InputField.gameObject.SetActive(true);
            if (m_SendButton != null) m_SendButton.gameObject.SetActive(true);
        }

        public void AutoScroll()
        {
            if (m_ScrollRect == null || m_IsUserScrolling)
            {
                return;
            }

            // Canvas縺ｮ譖ｴ譁ｰ繧貞ｾ・▲縺ｦ縺九ｉ繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ縺吶ｋ縺溘ａ縺ｫ繧ｳ繝ｫ繝ｼ繝√Φ縺矩≦蟒ｶ螳溯｡後ｒ菴ｿ縺・・縺御ｸ闊ｬ逧・□縺後・            // 縺薙％縺ｧ縺ｯ邁｡譏鍋噪縺ｫDOTween縺ｧ驕・ｻｶ縺輔○繧・            DOVirtual.DelayedCall(0.1f, () => {
                if(m_ScrollRect == null) return;
                
                // DOTween繧剃ｽｿ逕ｨ縺励◆繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・・.3遘抵ｼ・                DOTween.To(
                    () => m_ScrollRect.verticalNormalizedPosition,
                    x => m_ScrollRect.verticalNormalizedPosition = x,
                    0.0f, // 0.0f is bottom for vertical scroll rect
                    0.3f
                ).OnComplete(() =>
                {
                    // 繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ螳御ｺ・ｾ後↓m_LastScrollPosition繧呈峩譁ｰ
                    m_LastScrollPosition = 0.0f;
                });
            });
        }

        public void OnSubmit()
        {
            if (m_InputField == null) return;

            string text = m_InputField.text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                AddMessage("player", text);
                m_InputField.text = "";
                
                // 蜈･蜉帶ｬ・↓繝輔か繝ｼ繧ｫ繧ｹ繧呈綾縺・                m_InputField.ActivateInputField();
            }
        }

        /// <summary>
        /// 繝√Ε繝・ヨ螻･豁ｴ繧偵け繝ｪ繧｢
        /// </summary>
        public void ClearMessages()
        {
            if (m_ScrollRect == null || m_ScrollRect.content == null)
            {
                return;
            }

            // m_ScrollRect.content縺ｮ蟄舌が繝悶ず繧ｧ繧ｯ繝茨ｼ医Γ繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ・峨ｒ蜈ｨ縺ｦ蜑企勁
            int childCount = m_ScrollRect.content.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = m_ScrollRect.content.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
        #endregion
    }
}
