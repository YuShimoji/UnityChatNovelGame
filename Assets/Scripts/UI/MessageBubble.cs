using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 個々のメッセージバブルに付与されるコンポーネント
    /// 長押し検出による矛盾指摘モードの起点、タップによる2つ目の選択を担当
    /// </summary>
    public class MessageBubble : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private const float LongPressThreshold = 0.5f;

        private string m_LineTag;
        private float m_PointerDownTime;
        private bool m_IsLongPress;
        private Image m_BackgroundImage;
        private Color m_OriginalColor;
        private Tween m_HighlightTween;

        /// <summary>
        /// このバブルの矛盾判定用識別タグ
        /// </summary>
        public string LineTag
        {
            get => m_LineTag;
            set => m_LineTag = value;
        }

        /// <summary>
        /// 矛盾指摘の対象かどうか（LineTag が設定されているか）
        /// </summary>
        public bool IsPointable => !string.IsNullOrEmpty(m_LineTag);

        private void Awake()
        {
            m_BackgroundImage = GetComponent<Image>();
            if (m_BackgroundImage != null)
            {
                m_OriginalColor = m_BackgroundImage.color;
            }
        }

        /// <summary>
        /// LineTag と背景参照を初期化する
        /// </summary>
        public void Initialize(string lineTag, Image backgroundImage = null)
        {
            m_LineTag = lineTag;
            if (backgroundImage != null)
            {
                m_BackgroundImage = backgroundImage;
                m_OriginalColor = m_BackgroundImage.color;
            }
            else if (m_BackgroundImage != null)
            {
                m_OriginalColor = m_BackgroundImage.color;
            }
        }

        /// <summary>
        /// プールに返却される際にリセットする
        /// </summary>
        public void ResetState()
        {
            m_LineTag = null;
            m_IsLongPress = false;
            StopHighlight();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsPointable) return;

            var cm = ContradictionManager.Instance;
            if (cm == null || cm.IsOnCooldown) return;

            m_PointerDownTime = Time.unscaledTime;
            m_IsLongPress = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsPointable) return;
            // 長押し判定
            if (Time.unscaledTime - m_PointerDownTime >= LongPressThreshold && !m_IsLongPress)
            {
                m_IsLongPress = true;
                OnLongPress();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsPointable) return;

            var cm = ContradictionManager.Instance;
            if (cm == null) return;

            // 長押しの場合はクリックを無視
            if (m_IsLongPress) return;

            // 指摘モード中なら2つ目を選択
            if (cm.IsInPointingMode)
            {
                cm.SelectSecond(m_LineTag);
            }
        }

        private void OnLongPress()
        {
            var cm = ContradictionManager.Instance;
            if (cm == null || cm.IsOnCooldown) return;

            // 既に指摘モード中なら一旦キャンセル
            if (cm.IsInPointingMode)
            {
                cm.ClearSelection();
            }

            if (cm.SelectFirst(m_LineTag))
            {
                ShowHighlight();
            }
        }

        /// <summary>
        /// 選択中の脈動ハイライトを表示
        /// </summary>
        public void ShowHighlight()
        {
            if (m_BackgroundImage == null) return;

            StopHighlight();

            Color glowColor = new Color(0.3f, 0.6f, 1.0f, 0.8f);
            m_HighlightTween = m_BackgroundImage
                .DOColor(glowColor, 0.6f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }

        /// <summary>
        /// ハイライトを解除して元の色に戻す
        /// </summary>
        public void StopHighlight()
        {
            if (m_HighlightTween != null && m_HighlightTween.IsActive())
            {
                m_HighlightTween.Kill(false);
                m_HighlightTween = null;
            }

            if (m_BackgroundImage != null)
            {
                m_BackgroundImage.color = m_OriginalColor;
            }
        }

        private void OnDisable()
        {
            StopHighlight();
        }
    }
}
