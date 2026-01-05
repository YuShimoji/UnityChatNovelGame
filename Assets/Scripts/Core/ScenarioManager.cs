using UnityEngine;
using Yarn.Unity;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// Yarn SpinnerのDialogueRunnerをラップし、カスタムコマンドを処理するシナリオ管理クラス
    /// チャットシステムと連携してメッセージ表示やトピック解放などを制御する
    /// </summary>
    public class ScenarioManager : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private DialogueRunner m_DialogueRunner;
        [SerializeField] private ChatController m_ChatController;
        [SerializeField] private string m_StartNode = "Start";
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            RegisterCustomCommands();
            // TODO: 初期シナリオノードの開始処理
        }

        private void OnDestroy()
        {
            UnregisterCustomCommands();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 必要なコンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
            if (m_DialogueRunner == null)
            {
                m_DialogueRunner = GetComponent<DialogueRunner>();
            }

            if (m_DialogueRunner == null)
            {
                Debug.LogError("ScenarioManager: DialogueRunner component is required!");
            }

            if (m_ChatController == null)
            {
                m_ChatController = FindObjectOfType<ChatController>();
            }

            if (m_ChatController == null)
            {
                Debug.LogWarning("ScenarioManager: ChatController not found. Some features may not work.");
            }
        }

        /// <summary>
        /// Yarn Spinnerのカスタムコマンドを登録
        /// </summary>
        private void RegisterCustomCommands()
        {
            if (m_DialogueRunner == null)
            {
                return;
            }

            // TODO: DialogueRunnerにカスタムコマンドハンドラを登録
            // m_DialogueRunner.AddCommandHandler<...>("Message", MessageCommand);
            // m_DialogueRunner.AddCommandHandler<...>("Image", ImageCommand);
            // m_DialogueRunner.AddCommandHandler<...>("StartWait", StartWaitCommand);
            // m_DialogueRunner.AddCommandHandler<...>("UnlockTopic", UnlockTopicCommand);
            // m_DialogueRunner.AddCommandHandler<...>("Glitch", GlitchCommand);
        }

        /// <summary>
        /// カスタムコマンドの登録を解除
        /// </summary>
        private void UnregisterCustomCommands()
        {
            // TODO: 登録したコマンドハンドラを解除
        }
        #endregion

        #region Custom Command Handlers
        /// <summary>
        /// Messageコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<Message "CharID" "Text">>
        /// </summary>
        /// <param name="charID">キャラクターID</param>
        /// <param name="text">メッセージテキスト</param>
        private void MessageCommand(string charID, string text)
        {
            if (m_ChatController != null)
            {
                m_ChatController.AddMessage(charID, text);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. Message from {charID}: {text}");
            }
        }

        /// <summary>
        /// Imageコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<Image "CharID" "ImageID">>
        /// </summary>
        /// <param name="charID">キャラクターID</param>
        /// <param name="imageID">画像リソースのID</param>
        private void ImageCommand(string charID, string imageID)
        {
            // TODO: Resourcesフォルダから画像を読み込み
            // TODO: ChatControllerに画像メッセージとして送信
            Debug.Log($"ScenarioManager: Image command - CharID: {charID}, ImageID: {imageID}");
        }

        /// <summary>
        /// StartWaitコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<StartWait 15>>
        /// 指定秒数待機し、その間入力をロックする
        /// </summary>
        /// <param name="seconds">待機秒数</param>
        private void StartWaitCommand(int seconds)
        {
            // TODO: タイピングインジケーターを表示
            // TODO: 入力ロックを有効化
            // TODO: 指定秒数後に待機を解除
            // TODO: タイピングインジケーターを非表示
            Debug.Log($"ScenarioManager: StartWait command - {seconds} seconds");
        }

        /// <summary>
        /// UnlockTopicコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<UnlockTopic "TopicID">>
        /// 推論ボードに新しいトピックを追加する
        /// </summary>
        /// <param name="topicID">解放するトピックのID</param>
        private void UnlockTopicCommand(string topicID)
        {
            // TODO: ResourcesフォルダからTopicDataを読み込み
            // TODO: 推論ボード（DeductionBoard）にトピックを追加
            // TODO: Yarn変数を更新（例: $has_topic_{topicID} = true）
            Debug.Log($"ScenarioManager: UnlockTopic command - TopicID: {topicID}");
        }

        /// <summary>
        /// Glitchコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<Glitch 3>>
        /// 画面にノイズ演出を適用する
        /// </summary>
        /// <param name="level">グリッチの強度レベル（1-5程度を想定）</param>
        private void GlitchCommand(int level)
        {
            // TODO: MetaEffectControllerまたは専用のGlitchEffectコンポーネントにグリッチ効果を要求
            // TODO: レベルに応じた強度でノイズを表示
            Debug.Log($"ScenarioManager: Glitch command - Level: {level}");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// シナリオを開始
        /// </summary>
        /// <param name="nodeName">開始するYarnノード名（省略時はm_StartNodeを使用）</param>
        public void StartScenario(string nodeName = null)
        {
            if (m_DialogueRunner == null)
            {
                Debug.LogError("ScenarioManager: DialogueRunner is not initialized!");
                return;
            }

            string targetNode = nodeName ?? m_StartNode;
            // TODO: DialogueRunner.StartDialogue(targetNode)を呼び出し
        }

        /// <summary>
        /// シナリオを停止
        /// </summary>
        public void StopScenario()
        {
            if (m_DialogueRunner != null)
            {
                // TODO: DialogueRunner.Stop()を呼び出し
            }
        }

        /// <summary>
        /// Yarn変数の値を取得
        /// </summary>
        /// <typeparam name="T">変数の型</typeparam>
        /// <param name="variableName">変数名</param>
        /// <returns>変数の値</returns>
        public T GetVariable<T>(string variableName)
        {
            // TODO: DialogueRunner.VariableStorageから変数を取得
            return default(T);
        }

        /// <summary>
        /// Yarn変数の値を設定
        /// </summary>
        /// <typeparam name="T">変数の型</typeparam>
        /// <param name="variableName">変数名</param>
        /// <param name="value">設定する値</param>
        public void SetVariable<T>(string variableName, T value)
        {
            // TODO: DialogueRunner.VariableStorageに変数を設定
        }
        #endregion
    }
}
