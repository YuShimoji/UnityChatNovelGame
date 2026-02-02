using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Data;
using System;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 個別のセーブスロットUIを表すクラス
    /// </summary>
    public class SaveSlotUI : MonoBehaviour
    {
        #region Events
        /// <summary>
        /// スロットがクリックされた時のイベント
        /// </summary>
        public event Action<int> OnSlotClicked;

        /// <summary>
        /// 削除ボタンがクリックされた時のイベント
        /// </summary>
        public event Action<int> OnDeleteClicked;
        #endregion

        #region Private Fields
        [Header("UI References")]
        [SerializeField] private Button m_MainButton;
        [SerializeField] private Button m_DeleteButton;
        [SerializeField] private TextMeshProUGUI m_SlotNumberText;
        [SerializeField] private TextMeshProUGUI m_SaveInfoText;
        [SerializeField] private TextMeshProUGUI m_EmptySlotText;
        [SerializeField] private GameObject m_SaveDataPanel;
        [SerializeField] private GameObject m_EmptySlotPanel;

        private int m_SlotNumber;
        private SaveData m_SaveData;
        private SaveLoadUI.SaveLoadMode m_Mode;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (m_MainButton != null)
            {
                m_MainButton.onClick.AddListener(OnMainButtonClicked);
            }

            if (m_DeleteButton != null)
            {
                m_DeleteButton.onClick.AddListener(OnDeleteButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (m_MainButton != null)
            {
                m_MainButton.onClick.RemoveListener(OnMainButtonClicked);
            }

            if (m_DeleteButton != null)
            {
                m_DeleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// スロットUIをセットアップ
        /// </summary>
        /// <param name="slotNumber">スロット番号</param>
        /// <param name="saveData">セーブデータ（存在しない場合null）</param>
        /// <param name="mode">表示モード（Save/Load）</param>
        public void Setup(int slotNumber, SaveData saveData, SaveLoadUI.SaveLoadMode mode)
        {
            m_SlotNumber = slotNumber;
            m_SaveData = saveData;
            m_Mode = mode;

            UpdateUI();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// UIを更新
        /// </summary>
        private void UpdateUI()
        {
            if (m_SlotNumberText != null)
            {
                m_SlotNumberText.text = $"Slot {m_SlotNumber + 1}";
            }

            bool hasSaveData = m_SaveData != null;

            if (m_SaveDataPanel != null)
            {
                m_SaveDataPanel.SetActive(hasSaveData);
            }

            if (m_EmptySlotPanel != null)
            {
                m_EmptySlotPanel.SetActive(!hasSaveData);
            }

            if (hasSaveData)
            {
                if (m_SaveInfoText != null)
                {
                    m_SaveInfoText.text = m_SaveData.GetSummary();
                }

                if (m_DeleteButton != null)
                {
                    m_DeleteButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (m_EmptySlotText != null)
                {
                    m_EmptySlotText.text = m_Mode == SaveLoadUI.SaveLoadMode.Save 
                        ? "Empty Slot - Click to Save" 
                        : "Empty Slot";
                }

                if (m_DeleteButton != null)
                {
                    m_DeleteButton.gameObject.SetActive(false);
                }
            }

            if (m_MainButton != null)
            {
                m_MainButton.interactable = m_Mode == SaveLoadUI.SaveLoadMode.Save || hasSaveData;
            }
        }

        /// <summary>
        /// メインボタンがクリックされた時の処理
        /// </summary>
        private void OnMainButtonClicked()
        {
            OnSlotClicked?.Invoke(m_SlotNumber);
        }

        /// <summary>
        /// 削除ボタンがクリックされた時の処理
        /// </summary>
        private void OnDeleteButtonClicked()
        {
            OnDeleteClicked?.Invoke(m_SlotNumber);
        }
        #endregion
    }
}
