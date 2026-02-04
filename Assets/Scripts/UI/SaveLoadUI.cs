using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 繧ｻ繝ｼ繝悶・繝ｭ繝ｼ繝蔚I繧堤ｮ｡逅・☆繧九け繝ｩ繧ｹ
    /// 繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨ縺ｮ陦ｨ遉ｺ縲√そ繝ｼ繝・繝ｭ繝ｼ繝・蜑企勁謫堺ｽ懊ｒ謠蝉ｾ・    /// </summary>
    public class SaveLoadUI : MonoBehaviour
    {
        #region Private Fields
        [Header("UI References")]
        [SerializeField] private GameObject m_Panel;
        [SerializeField] private Transform m_SlotContainer;
        [SerializeField] private SaveSlotUI m_SlotPrefab;
        [SerializeField] private Button m_CloseButton;

        [Header("Mode Settings")]
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private string m_SaveModeTitle = "Save Game";
        [SerializeField] private string m_LoadModeTitle = "Load Game";

        private List<SaveSlotUI> m_SlotUIs = new List<SaveSlotUI>();
        private SaveLoadMode m_CurrentMode = SaveLoadMode.Save;
        #endregion

        #region Enums
        public enum SaveLoadMode
        {
            Save,
            Load
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (m_CloseButton != null)
            {
                m_CloseButton.onClick.AddListener(Hide);
            }

            if (m_Panel != null)
            {
                m_Panel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (m_CloseButton != null)
            {
                m_CloseButton.onClick.RemoveListener(Hide);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 繧ｻ繝ｼ繝悶Δ繝ｼ繝峨〒UI繧定｡ｨ遉ｺ
        /// </summary>
        public void ShowSaveMode()
        {
            m_CurrentMode = SaveLoadMode.Save;
            if (m_TitleText != null)
            {
                m_TitleText.text = m_SaveModeTitle;
            }
            RefreshSlots();
            Show();
        }

        /// <summary>
        /// 繝ｭ繝ｼ繝峨Δ繝ｼ繝峨〒UI繧定｡ｨ遉ｺ
        /// </summary>
        public void ShowLoadMode()
        {
            m_CurrentMode = SaveLoadMode.Load;
            if (m_TitleText != null)
            {
                m_TitleText.text = m_LoadModeTitle;
            }
            RefreshSlots();
            Show();
        }

        /// <summary>
        /// UI繧定｡ｨ遉ｺ
        /// </summary>
        public void Show()
        {
            if (m_Panel != null)
            {
                m_Panel.SetActive(true);
            }
        }

        /// <summary>
        /// UI繧帝撼陦ｨ遉ｺ
        /// </summary>
        public void Hide()
        {
            if (m_Panel != null)
            {
                m_Panel.SetActive(false);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨUI繧呈峩譁ｰ
        /// </summary>
        private void RefreshSlots()
        {
            foreach (var slotUI in m_SlotUIs)
            {
                if (slotUI != null)
                {
                    Destroy(slotUI.gameObject);
                }
            }
            m_SlotUIs.Clear();

            if (SaveManager.Instance == null)
            {
                Debug.LogError("SaveLoadUI: SaveManager instance not found.");
                return;
            }

            SaveData[] allSaves = SaveManager.Instance.GetAllSaveInfo();
            for (int i = 0; i < allSaves.Length; i++)
            {
                CreateSlotUI(i, allSaves[i]);
            }
        }

        /// <summary>
        /// 繧ｻ繝ｼ繝悶せ繝ｭ繝・ヨUI繧堤函謌・        /// </summary>
        /// <param name="slotNumber">繧ｹ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        /// <param name="saveData">繧ｻ繝ｼ繝悶ョ繝ｼ繧ｿ・亥ｭ伜惠縺励↑縺・ｴ蜷・ull・・/param>
        private void CreateSlotUI(int slotNumber, SaveData saveData)
        {
            if (m_SlotPrefab == null || m_SlotContainer == null)
            {
                Debug.LogError("SaveLoadUI: Slot prefab or container not assigned.");
                return;
            }

            SaveSlotUI slotUI = Instantiate(m_SlotPrefab, m_SlotContainer);
            slotUI.Setup(slotNumber, saveData, m_CurrentMode);
            slotUI.OnSlotClicked += OnSlotClicked;
            slotUI.OnDeleteClicked += OnDeleteClicked;
            m_SlotUIs.Add(slotUI);
        }

        /// <summary>
        /// 繧ｹ繝ｭ繝・ヨ縺後け繝ｪ繝・け縺輔ｌ縺滓凾縺ｮ蜃ｦ逅・        /// </summary>
        /// <param name="slotNumber">繧ｯ繝ｪ繝・け縺輔ｌ縺溘せ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        private void OnSlotClicked(int slotNumber)
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogError("SaveLoadUI: SaveManager instance not found.");
                return;
            }

            bool success = false;
            if (m_CurrentMode == SaveLoadMode.Save)
            {
                success = SaveManager.Instance.SaveGame(slotNumber);
                if (success)
                {
                    Debug.Log($"SaveLoadUI: Game saved to slot {slotNumber}");
                    RefreshSlots();
                }
            }
            else if (m_CurrentMode == SaveLoadMode.Load)
            {
                success = SaveManager.Instance.LoadGame(slotNumber);
                if (success)
                {
                    Debug.Log($"SaveLoadUI: Game loaded from slot {slotNumber}");
                    Hide();
                }
            }

            if (!success)
            {
                Debug.LogWarning($"SaveLoadUI: Operation failed for slot {slotNumber}");
            }
        }

        /// <summary>
        /// 蜑企勁繝懊ち繝ｳ縺後け繝ｪ繝・け縺輔ｌ縺滓凾縺ｮ蜃ｦ逅・        /// </summary>
        /// <param name="slotNumber">蜑企勁縺吶ｋ繧ｹ繝ｭ繝・ヨ逡ｪ蜿ｷ</param>
        private void OnDeleteClicked(int slotNumber)
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogError("SaveLoadUI: SaveManager instance not found.");
                return;
            }

            bool success = SaveManager.Instance.DeleteSave(slotNumber);
            if (success)
            {
                Debug.Log($"SaveLoadUI: Save deleted from slot {slotNumber}");
                RefreshSlots();
            }
            else
            {
                Debug.LogWarning($"SaveLoadUI: Failed to delete save from slot {slotNumber}");
            }
        }
        #endregion
    }
}
