using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 矛盾指摘システムのビジュアルフィードバックを統括するコントローラー。
    /// ContradictionManager の4イベントを購読し、バブルハイライト・接続線・
    /// 成功/失敗アニメーション・発見通知を制御する。
    /// Canvas 直下に配置し、ChatController への参照を Inspector でアサインすること。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ContradictionFeedbackController : MonoBehaviour
    {
        [SerializeField] private ChatController m_ChatController;

        #region State
        private string m_CachedFirstLineTag;
        private MessageBubble m_CachedFirstBubble;
        private bool m_IsPlayingResultAnimation;
        private Canvas m_Canvas;
        private Camera m_CanvasCamera;
        #endregion

        #region UI Elements (dynamic)
        private GameObject m_HintBanner;
        private CanvasGroup m_HintBannerGroup;
        private TextMeshProUGUI m_HintText;

        private RectTransform m_LineRect;
        private Image m_LineImage;

        private GameObject m_NotificationPanel;
        private CanvasGroup m_NotificationGroup;
        private RectTransform m_NotificationRect;
        private TextMeshProUGUI m_NotificationTitle;
        private TextMeshProUGUI m_NotificationReward;
        #endregion

        #region DOTween
        private Sequence m_ResultSequence;
        private Tween m_BannerTween;
        private Sequence m_NotificationSequence;
        #endregion

        #region Constants
        private static readonly Color SuccessColor = new Color(0.2f, 0.85f, 0.4f, 0.8f);
        private static readonly Color FailNoMatchColor = new Color(0.9f, 0.2f, 0.2f, 0.7f);
        private static readonly Color FailAlreadyColor = new Color(0.9f, 0.75f, 0.2f, 0.6f);
        private static readonly Color LineColor = new Color(0.4f, 0.7f, 1.0f, 0.5f);
        private static readonly Color BannerBgColor = new Color(0.15f, 0.15f, 0.2f, 0.85f);
        private static readonly Color NotifBgColor = new Color(0.1f, 0.1f, 0.15f, 0.92f);
        private static readonly Color RewardTextColor = new Color(1f, 0.85f, 0.3f, 1f);
        private const float FlashDuration = 0.4f;
        private const float PulseDuration = 0.5f;
        private const float PulseScale = 1.08f;
        private const float LineWidth = 3f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // RectTransform を親いっぱいに拡張
            var rt = GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas != null && m_Canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                m_CanvasCamera = m_Canvas.worldCamera;

            CreateHintBanner();
            CreateConnectionLine();
            CreateNotificationPanel();
        }

        private void OnEnable()
        {
            var cm = ContradictionManager.Instance;
            if (cm == null) return;

            cm.OnSelectionChanged += HandleSelectionChanged;
            cm.OnSelectionCleared += HandleSelectionCleared;
            cm.OnContradictionDiscovered += HandleContradictionDiscovered;
            cm.OnPointingFailed += HandlePointingFailed;
        }

        private void OnDisable()
        {
            var cm = ContradictionManager.Instance;
            if (cm == null) return;

            cm.OnSelectionChanged -= HandleSelectionChanged;
            cm.OnSelectionCleared -= HandleSelectionCleared;
            cm.OnContradictionDiscovered -= HandleContradictionDiscovered;
            cm.OnPointingFailed -= HandlePointingFailed;
        }

        private void OnDestroy()
        {
            m_ResultSequence?.Kill();
            m_BannerTween?.Kill();
            m_NotificationSequence?.Kill();
        }
        #endregion

        #region UI Creation
        private void CreateHintBanner()
        {
            m_HintBanner = new GameObject("HintBanner", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            m_HintBanner.transform.SetParent(transform, false);

            var rt = m_HintBanner.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 1f);
            rt.anchorMax = new Vector2(0.9f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -10f);
            rt.sizeDelta = new Vector2(0f, 36f);

            m_HintBanner.GetComponent<Image>().color = BannerBgColor;
            m_HintBannerGroup = m_HintBanner.GetComponent<CanvasGroup>();
            m_HintBannerGroup.alpha = 0f;
            m_HintBannerGroup.blocksRaycasts = false;

            var textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(m_HintBanner.transform, false);
            var textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(10f, 0f);
            textRT.offsetMax = new Vector2(-10f, 0f);

            m_HintText = textGO.GetComponent<TextMeshProUGUI>();
            m_HintText.fontSize = 14f;
            m_HintText.color = Color.white;
            m_HintText.alignment = TextAlignmentOptions.Center;
            m_HintText.enableWordWrapping = false;

            m_HintBanner.SetActive(false);
        }

        private void CreateConnectionLine()
        {
            var go = new GameObject("ConnectionLine", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);

            m_LineRect = go.GetComponent<RectTransform>();
            m_LineRect.pivot = new Vector2(0f, 0.5f);
            m_LineRect.anchorMin = new Vector2(0.5f, 0.5f);
            m_LineRect.anchorMax = new Vector2(0.5f, 0.5f);
            m_LineRect.sizeDelta = new Vector2(0f, LineWidth);

            m_LineImage = go.GetComponent<Image>();
            m_LineImage.color = LineColor;
            m_LineImage.raycastTarget = false;

            go.SetActive(false);
        }

        private void CreateNotificationPanel()
        {
            m_NotificationPanel = new GameObject("NotificationPanel", typeof(RectTransform), typeof(CanvasGroup));
            m_NotificationPanel.transform.SetParent(transform, false);

            m_NotificationRect = m_NotificationPanel.GetComponent<RectTransform>();
            m_NotificationRect.anchorMin = new Vector2(0.5f, 0.5f);
            m_NotificationRect.anchorMax = new Vector2(0.5f, 0.5f);
            m_NotificationRect.sizeDelta = new Vector2(300f, 120f);

            m_NotificationGroup = m_NotificationPanel.GetComponent<CanvasGroup>();
            m_NotificationGroup.alpha = 0f;
            m_NotificationGroup.blocksRaycasts = false;

            // Background
            var bgGO = new GameObject("Bg", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(m_NotificationPanel.transform, false);
            var bgRT = bgGO.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            bgGO.GetComponent<Image>().color = NotifBgColor;

            // Title
            var titleGO = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleGO.transform.SetParent(m_NotificationPanel.transform, false);
            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0f, 0.5f);
            titleRT.anchorMax = new Vector2(1f, 1f);
            titleRT.offsetMin = new Vector2(16f, 8f);
            titleRT.offsetMax = new Vector2(-16f, -12f);

            m_NotificationTitle = titleGO.GetComponent<TextMeshProUGUI>();
            m_NotificationTitle.fontSize = 20f;
            m_NotificationTitle.color = Color.white;
            m_NotificationTitle.alignment = TextAlignmentOptions.Center;

            // Reward
            var rewardGO = new GameObject("Reward", typeof(RectTransform), typeof(TextMeshProUGUI));
            rewardGO.transform.SetParent(m_NotificationPanel.transform, false);
            var rewardRT = rewardGO.GetComponent<RectTransform>();
            rewardRT.anchorMin = new Vector2(0f, 0f);
            rewardRT.anchorMax = new Vector2(1f, 0.5f);
            rewardRT.offsetMin = new Vector2(16f, 12f);
            rewardRT.offsetMax = new Vector2(-16f, -8f);

            m_NotificationReward = rewardGO.GetComponent<TextMeshProUGUI>();
            m_NotificationReward.fontSize = 18f;
            m_NotificationReward.color = RewardTextColor;
            m_NotificationReward.alignment = TextAlignmentOptions.Center;

            m_NotificationPanel.SetActive(false);
        }
        #endregion

        #region Event Handlers
        private void HandleSelectionChanged(string lineTag)
        {
            // 再選択時に前のバブルをクリーンアップ（潜在バグ修正）
            if (m_CachedFirstBubble != null)
            {
                m_CachedFirstBubble.StopHighlight();
            }

            m_CachedFirstLineTag = lineTag;
            m_CachedFirstBubble = m_ChatController != null
                ? m_ChatController.FindBubbleByLineTag(lineTag)
                : null;
            m_IsPlayingResultAnimation = false;

            ShowHintBanner("2つ目のメッセージをタップしてください");
        }

        private void HandleSelectionCleared()
        {
            // 成功/失敗アニメーション中は StopHighlight を呼ばない（アニメが管理する）
            if (!m_IsPlayingResultAnimation && m_CachedFirstBubble != null)
            {
                m_CachedFirstBubble.StopHighlight();
            }

            m_CachedFirstLineTag = null;
            m_CachedFirstBubble = null;

            HideHintBanner();
            HideConnectionLine();
        }

        private void HandleContradictionDiscovered(ContradictionPair pair)
        {
            m_IsPlayingResultAnimation = true;

            // 両バブルの特定（キャッシュが pair に含まれない場合の防御）
            string secondTag;
            if (m_CachedFirstLineTag == pair.SourceLineTag)
                secondTag = pair.TargetLineTag;
            else if (m_CachedFirstLineTag == pair.TargetLineTag)
                secondTag = pair.SourceLineTag;
            else
            {
                Debug.LogWarning($"ContradictionFeedbackController: cached tag '{m_CachedFirstLineTag}' does not match pair.");
                secondTag = pair.TargetLineTag;
            }

            MessageBubble firstBubble = m_CachedFirstBubble;
            MessageBubble secondBubble = m_ChatController?.FindBubbleByLineTag(secondTag);

            // ハイライト + 既存 DOTween を停止してからアニメーション開始
            KillBubbleTweens(firstBubble);
            KillBubbleTweens(secondBubble);

            if (firstBubble != null && secondBubble != null)
                ShowConnectionLine(firstBubble, secondBubble);

            PlaySuccessAnimation(firstBubble, secondBubble, pair);
        }

        private void HandlePointingFailed(string reason)
        {
            m_IsPlayingResultAnimation = true;

            MessageBubble bubble = m_CachedFirstBubble;
            if (bubble == null) return;

            KillBubbleTweens(bubble);

            if (reason == "already_discovered")
            {
                PlayFailAnimation(bubble, FailAlreadyColor, "既に発見済みです");
            }
            else
            {
                PlayFailAnimation(bubble, FailNoMatchColor, "矛盾が見つかりませんでした");
            }
        }
        /// <summary>
        /// バブルのハイライト停止 + Image 上の DOTween を確実に停止する
        /// </summary>
        private void KillBubbleTweens(MessageBubble bubble)
        {
            if (bubble == null) return;
            bubble.StopHighlight();
            var img = bubble.GetComponent<Image>();
            if (img != null)
                DOTween.Kill(img, complete: false);
        }
        #endregion

        #region Hint Banner
        private void ShowHintBanner(string message)
        {
            m_BannerTween?.Kill();
            m_HintBanner.SetActive(true);
            m_HintText.text = message;
            m_HintBannerGroup.alpha = 0f;
            m_BannerTween = m_HintBannerGroup.DOFade(1f, 0.3f).SetUpdate(true);
        }

        private void ShowErrorBanner(string message)
        {
            m_BannerTween?.Kill();
            m_HintBanner.SetActive(true);
            m_HintText.text = message;
            m_HintBannerGroup.alpha = 1f;

            m_BannerTween = DOTween.Sequence()
                .AppendInterval(2f)
                .Append(m_HintBannerGroup.DOFade(0f, 0.3f))
                .OnComplete(() => m_HintBanner.SetActive(false))
                .SetUpdate(true);
        }

        private void HideHintBanner()
        {
            // エラーバナー表示中（result animation 後）はそのまま残す
            if (m_IsPlayingResultAnimation) return;

            m_BannerTween?.Kill();
            if (m_HintBanner.activeSelf)
            {
                m_BannerTween = m_HintBannerGroup.DOFade(0f, 0.2f)
                    .SetUpdate(true)
                    .OnComplete(() => m_HintBanner.SetActive(false));
            }
        }
        #endregion

        #region Connection Line
        private void ShowConnectionLine(MessageBubble a, MessageBubble b)
        {
            if (m_LineRect == null) return;

            RectTransform parentRect = (RectTransform)transform;
            Vector2 screenA = RectTransformUtility.WorldToScreenPoint(m_CanvasCamera, a.transform.position);
            Vector2 screenB = RectTransformUtility.WorldToScreenPoint(m_CanvasCamera, b.transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenA, m_CanvasCamera, out Vector2 localA);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, screenB, m_CanvasCamera, out Vector2 localB);

            Vector2 dir = localB - localA;
            float dist = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            m_LineRect.anchoredPosition = localA;
            m_LineRect.sizeDelta = new Vector2(dist, LineWidth);
            m_LineRect.localRotation = Quaternion.Euler(0f, 0f, angle);
            m_LineImage.color = LineColor;
            m_LineRect.gameObject.SetActive(true);
        }

        private void HideConnectionLine()
        {
            if (m_LineRect != null)
                m_LineRect.gameObject.SetActive(false);
        }

        private void FadeOutConnectionLine(Color flashColor, float duration)
        {
            if (m_LineRect == null || !m_LineRect.gameObject.activeSelf) return;

            DOTween.Sequence()
                .Append(m_LineImage.DOColor(flashColor, 0.15f))
                .AppendInterval(0.1f)
                .Append(m_LineImage.DOFade(0f, duration))
                .OnComplete(() => m_LineRect.gameObject.SetActive(false))
                .SetUpdate(true);
        }
        #endregion

        #region Success Animation
        private void PlaySuccessAnimation(MessageBubble first, MessageBubble second, ContradictionPair pair)
        {
            m_ResultSequence?.Kill();
            m_ResultSequence = DOTween.Sequence().SetUpdate(true);

            AnimateBubbleFlash(first, SuccessColor, m_ResultSequence);
            AnimateBubbleFlash(second, SuccessColor, m_ResultSequence);

            FadeOutConnectionLine(SuccessColor, 0.5f);

            m_ResultSequence.AppendCallback(() => ShowNotification(pair));
            m_ResultSequence.OnComplete(() => m_IsPlayingResultAnimation = false);
        }

        private void AnimateBubbleFlash(MessageBubble bubble, Color flashColor, Sequence seq)
        {
            if (bubble == null) return;

            Image img = bubble.GetComponent<Image>();
            if (img == null) return;

            // 色フラッシュ: 現在色→flashColor→現在色 (Yoyo 2ループ)
            seq.Join(img.DOColor(flashColor, FlashDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad));

            // スケールパルス: 1.0→PulseScale→1.0
            seq.Join(bubble.transform
                .DOScale(PulseScale, PulseDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutBack));
        }
        #endregion

        #region Failure Animation
        private void PlayFailAnimation(MessageBubble bubble, Color flashColor, string message)
        {
            m_ResultSequence?.Kill();
            m_ResultSequence = DOTween.Sequence().SetUpdate(true);

            Image img = bubble.GetComponent<Image>();
            if (img != null)
            {
                m_ResultSequence.Join(img.DOColor(flashColor, FlashDuration * 0.5f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad));
            }

            // 不一致の場合はシェイク（回転で表現、レイアウト競合を回避）
            if (flashColor == FailNoMatchColor)
            {
                m_ResultSequence.Join(bubble.transform
                    .DOShakeRotation(0.3f, new Vector3(0f, 0f, 5f), 20, 90f)
                    .SetUpdate(true));
            }

            FadeOutConnectionLine(flashColor, 0.3f);

            m_ResultSequence.AppendCallback(() => ShowErrorBanner(message));
            m_ResultSequence.OnComplete(() => m_IsPlayingResultAnimation = false);
        }
        #endregion

        #region Notification Panel
        private void ShowNotification(ContradictionPair pair)
        {
            m_NotificationSequence?.Kill();

            m_NotificationTitle.text = "矛盾を発見!";
            m_NotificationReward.text = $"+{pair.RewardCoin} HalluciCoin";

            m_NotificationPanel.SetActive(true);
            m_NotificationGroup.alpha = 0f;
            m_NotificationRect.anchoredPosition = new Vector2(0f, -200f);

            m_NotificationSequence = DOTween.Sequence().SetUpdate(true);
            m_NotificationSequence.Append(
                m_NotificationRect.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack));
            m_NotificationSequence.Join(
                m_NotificationGroup.DOFade(1f, 0.3f));
            m_NotificationSequence.AppendInterval(3f);
            m_NotificationSequence.Append(
                m_NotificationRect.DOAnchorPosY(-200f, 0.3f).SetEase(Ease.InBack));
            m_NotificationSequence.Join(
                m_NotificationGroup.DOFade(0f, 0.2f));
            m_NotificationSequence.OnComplete(() => m_NotificationPanel.SetActive(false));
        }
        #endregion
    }
}
