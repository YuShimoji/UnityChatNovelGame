using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// ダッシュボード上部に表示するコンパクトな進捗サマリー。
    /// プログレスバー + ミニ数値 + ヒントテキスト。
    /// </summary>
    public class ProgressSummaryUI : MonoBehaviour
    {
        private Image m_BarFill;
        private TextMeshProUGUI m_PercentText;
        private TextMeshProUGUI m_MiniStats;
        private TextMeshProUGUI m_NudgeText;

        /// <summary>
        /// UI 要素を構築する。DashboardController.BuildDashboardUI() から呼ばれる。
        /// </summary>
        public void Build(Transform parent, System.Action<TextMeshProUGUI> assignFont)
        {
            RectTransform root = GetComponent<RectTransform>();
            root.anchorMin = new Vector2(0f, 1f);
            root.anchorMax = new Vector2(1f, 1f);
            root.pivot = new Vector2(0.5f, 1f);
            root.offsetMin = new Vector2(40f, -145f);
            root.offsetMax = new Vector2(-40f, -100f);

            // --- プログレスバー背景 ---
            GameObject barBg = new GameObject("BarBg", typeof(RectTransform), typeof(Image));
            barBg.transform.SetParent(transform, false);
            barBg.layer = gameObject.layer;

            RectTransform barBgRect = barBg.GetComponent<RectTransform>();
            barBgRect.anchorMin = new Vector2(0f, 1f);
            barBgRect.anchorMax = new Vector2(0.75f, 1f);
            barBgRect.pivot = new Vector2(0f, 1f);
            barBgRect.offsetMin = new Vector2(0f, -8f);
            barBgRect.offsetMax = new Vector2(0f, 0f);

            Image barBgImg = barBg.GetComponent<Image>();
            barBgImg.color = new Color(0.15f, 0.15f, 0.18f, 0.8f);
            barBgImg.raycastTarget = false;

            // --- プログレスバー前景 (fill) ---
            GameObject barFill = new GameObject("BarFill", typeof(RectTransform), typeof(Image));
            barFill.transform.SetParent(barBg.transform, false);
            barFill.layer = gameObject.layer;

            RectTransform fillRect = barFill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            m_BarFill = barFill.GetComponent<Image>();
            m_BarFill.color = new Color(0.8f, 0.75f, 0.4f, 0.9f);
            m_BarFill.type = Image.Type.Filled;
            m_BarFill.fillMethod = Image.FillMethod.Horizontal;
            m_BarFill.fillAmount = 0f;
            m_BarFill.raycastTarget = false;

            // --- パーセント表示 ---
            GameObject pctObj = new GameObject("Percent", typeof(RectTransform));
            pctObj.transform.SetParent(transform, false);
            pctObj.layer = gameObject.layer;

            RectTransform pctRect = pctObj.GetComponent<RectTransform>();
            pctRect.anchorMin = new Vector2(0.78f, 1f);
            pctRect.anchorMax = new Vector2(1f, 1f);
            pctRect.pivot = new Vector2(1f, 1f);
            pctRect.offsetMin = new Vector2(0f, -10f);
            pctRect.offsetMax = new Vector2(0f, 0f);

            m_PercentText = pctObj.AddComponent<TextMeshProUGUI>();
            m_PercentText.text = "0%";
            m_PercentText.fontSize = 14f;
            m_PercentText.color = new Color(0.8f, 0.75f, 0.4f);
            m_PercentText.alignment = TextAlignmentOptions.MidlineRight;
            m_PercentText.raycastTarget = false;
            assignFont?.Invoke(m_PercentText);

            // --- ミニ数値 ---
            GameObject statsObj = new GameObject("MiniStats", typeof(RectTransform));
            statsObj.transform.SetParent(transform, false);
            statsObj.layer = gameObject.layer;

            RectTransform statsRect = statsObj.GetComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0f, 1f);
            statsRect.anchorMax = new Vector2(1f, 1f);
            statsRect.pivot = new Vector2(0.5f, 1f);
            statsRect.offsetMin = new Vector2(0f, -24f);
            statsRect.offsetMax = new Vector2(0f, -10f);

            m_MiniStats = statsObj.AddComponent<TextMeshProUGUI>();
            m_MiniStats.text = "";
            m_MiniStats.fontSize = 11f;
            m_MiniStats.color = new Color(0.45f, 0.45f, 0.5f);
            m_MiniStats.alignment = TextAlignmentOptions.Center;
            m_MiniStats.raycastTarget = false;
            assignFont?.Invoke(m_MiniStats);

            // --- ヒントテキスト ---
            GameObject nudgeObj = new GameObject("NudgeText", typeof(RectTransform));
            nudgeObj.transform.SetParent(transform, false);
            nudgeObj.layer = gameObject.layer;

            RectTransform nudgeRect = nudgeObj.GetComponent<RectTransform>();
            nudgeRect.anchorMin = new Vector2(0f, 1f);
            nudgeRect.anchorMax = new Vector2(1f, 1f);
            nudgeRect.pivot = new Vector2(0.5f, 1f);
            nudgeRect.offsetMin = new Vector2(0f, -40f);
            nudgeRect.offsetMax = new Vector2(0f, -24f);

            m_NudgeText = nudgeObj.AddComponent<TextMeshProUGUI>();
            m_NudgeText.text = "";
            m_NudgeText.fontSize = 10f;
            m_NudgeText.fontStyle = FontStyles.Italic;
            m_NudgeText.color = new Color(0.35f, 0.35f, 0.4f);
            m_NudgeText.alignment = TextAlignmentOptions.Center;
            m_NudgeText.raycastTarget = false;
            assignFont?.Invoke(m_NudgeText);
        }

        /// <summary>スナップショットとヒント文でUI表示を更新する</summary>
        public void UpdateDisplay(ProgressSnapshot snapshot, string nudge)
        {
            if (m_BarFill != null)
                m_BarFill.fillAmount = snapshot.OverallPercent;

            if (m_PercentText != null)
                m_PercentText.text = $"{Mathf.RoundToInt(snapshot.OverallPercent * 100f)}%";

            if (m_MiniStats != null)
            {
                m_MiniStats.text = string.Format(
                    "Ch {0}/{1}  |  Cont {2}/{3}  |  Frag {4}/{5}",
                    snapshot.ChaptersCompleted, snapshot.ChaptersTotal,
                    snapshot.ContradictionsFound, snapshot.ContradictionsTotal,
                    snapshot.FragmentsCollected, snapshot.FragmentsTotal);
            }

            if (m_NudgeText != null)
                m_NudgeText.text = nudge ?? "";
        }
    }
}
