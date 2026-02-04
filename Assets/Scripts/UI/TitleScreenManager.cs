using UnityEngine;
using UnityEngine.SceneManagement;
using ProjectFoundPhone.Core;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// タイトル画面の処理を管理するクラス
    /// ニューゲーム、ロード、オプション、終了などの操作を提供
    /// </summary>
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string m_NewGameSceneName = "DebugChatScene";

        [Header("UI References")]
        [SerializeField] private SaveLoadUI m_SaveLoadUI;
        [SerializeField] private GameObject m_OptionsPanel;

        /// <summary>
        /// ニューゲームを開始
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("TitleScreenManager: Starting New Game...");
            
            // セーブデータがある場合にリセットするかどうかの確認などは将来的に追加
            // 現状は即座にシーン遷移
            SceneManager.LoadScene(m_NewGameSceneName);
        }

        /// <summary>
        /// ロードメニューを開く
        /// </summary>
        public void OpenLoadMenu()
        {
            if (m_SaveLoadUI != null)
            {
                Debug.Log("TitleScreenManager: Opening Load Menu...");
                m_SaveLoadUI.ShowLoadMode();
            }
            else
            {
                Debug.LogError("TitleScreenManager: SaveLoadUI reference is missing!");
            }
        }

        /// <summary>
        /// オプションメニューを開く
        /// </summary>
        public void OpenOptions()
        {
            if (m_OptionsPanel != null)
            {
                Debug.Log("TitleScreenManager: Opening Options Panel...");
                m_OptionsPanel.SetActive(true);
            }
            else
            {
                Debug.Log("TitleScreenManager: Options functionality not implemented yet.");
            }
        }

        /// <summary>
        /// ゲームを終了
        /// </summary>
        public void ExitGame()
        {
            Debug.Log("TitleScreenManager: Exiting Game...");
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
