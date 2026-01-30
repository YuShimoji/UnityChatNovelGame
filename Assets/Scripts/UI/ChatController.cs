using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// メッセージ表示システム
    /// UI制御を行うコントローラー
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ChatController : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private VerticalLayoutGroup m_LayoutGroup;        
        [SerializeField] private GameObject m_MessageBubblePrefab;
        [SerializeField] private GameObject m_TypingIndicator;
        #endregion

        public void AddMessage(string message, bool isPlayer)
        {
            // Implementation placeholder to fix syntax error
            if (m_MessageBubblePrefab != null && m_LayoutGroup != null)
            {
                // Instantiate bubble
            }
        }

        public void ShowTyping(bool show)
        {
            if (m_TypingIndicator != null)
            {
                m_TypingIndicator.SetActive(show);
            }
        }
    }
}
