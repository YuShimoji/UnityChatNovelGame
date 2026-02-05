using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Data;
using System;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 蛟句挨縺ｮ繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨUI繧定｡ｨ縺吶け繝ｩ繧ｹ
    /// </summary>
    public class SaveSlotUI : MonoBehaviour
    {
        #region Events
        /// <summary>
        /// 繧ｹ繝ｭ繝・ヨ縺後け繝ｪ繝・け縺輔ｌ縺滓凾縺ｮ繧､繝吶Φ繝・        /// </summary>
        public event Action<int> OnSlotClicked;

        /// <summary>
        /// 蜑企勁繝懊ち繝ｳ縺後け繝ｪ繝・け縺輔ｌ縺滓凾縺ｮ繧､繝吶Φ繝・        /// </summary>
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
        /// 繧ｹ繝ｭ繝・ヨUI繧偵そ繝・ヨ繧｢繝・・
        /// </summary>
        /// <param name="slotNumber">繧ｹ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <param name="saveData">繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ・亥ｭ伜惠縺励↑縺・ｴ蜷・ull・・/param>
        /// <param name="mode">陦ｨ遉ｺ繝｢繝ｼ繝会ｼ・ave/Load・・/param>
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
        /// UI繧呈峩譁ｰ
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
        /// 繝｡繧､繝ｳ繝懊ち繝ｳ縺後け繝ｪ繝・け縺輔ｌ縺滓凾縺ｮ蜃ｦ逅・        /// </summary>
        private void OnMainButtonClicked()
        {
            OnSlotClicked?.Invoke(m_SlotNumber);
        }

        /// <summary>
        /// 蜑企勁繝懊ち繝ｳ縺後け繝ｪ繝・け縺輔ｌ縺滓凾縺ｮ蜃ｦ逅・        /// </summary>
        private void OnDeleteButtonClicked()
        {
            OnDeleteClicked?.Invoke(m_SlotNumber);
        }
        #endregion
    }
}
