using UnityEngine;
using System.Collections;
using Yarn.Unity;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data;
using DG.Tweening;

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

        private bool m_IsInputLocked = false;
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

            // DialogueRunnerにカスタムコマンドハンドラを登録
            // Yarn Spinnerのコマンドハンドラは通常、string[]配列で引数を受け取る
            m_DialogueRunner.AddCommandHandler<string, string>("Message", MessageCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("Image", ImageCommand);
            m_DialogueRunner.AddCommandHandler<int>("StartWait", StartWaitCommand);
            m_DialogueRunner.AddCommandHandler<string>("UnlockTopic", UnlockTopicCommand);
            m_DialogueRunner.AddCommandHandler<int>("Glitch", GlitchCommand);
        }

        /// <summary>
        /// カスタムコマンドの登録を解除
        /// </summary>
        private void UnregisterCustomCommands()
        {
            if (m_DialogueRunner == null)
            {
                return;
            }

            // 登録したコマンドハンドラを解除
            m_DialogueRunner.RemoveCommandHandler("Message");
            m_DialogueRunner.RemoveCommandHandler("Image");
            m_DialogueRunner.RemoveCommandHandler("StartWait");
            m_DialogueRunner.RemoveCommandHandler("UnlockTopic");
            m_DialogueRunner.RemoveCommandHandler("Glitch");
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
            // Resourcesフォルダから画像を読み込み
            Sprite imageSprite = Resources.Load<Sprite>($"Images/{imageID}");
            
            if (imageSprite == null)
            {
                Debug.LogWarning($"ScenarioManager: Failed to load image from Resources/Images/{imageID}");
                return;
            }

            // ChatControllerに画像メッセージとして送信
            // 現在のAddMessage()はテキストのみ対応のため、画像IDを含むテキストとして送信
            // 後続タスクで画像メッセージ専用のメソッドを追加する予定
            if (m_ChatController != null)
            {
                m_ChatController.AddMessage(charID, $"[Image: {imageID}]");
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. Image from {charID}: {imageID}");
            }
        }

        /// <summary>
        /// StartWaitコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<StartWait 15>>
        /// 指定秒数待機し、その間入力をロックする
        /// </summary>
        /// <param name="seconds">待機秒数</param>
        private void StartWaitCommand(int seconds)
        {
            // タイピングインジケーターを表示
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(true);
            }

            // 入力ロックを有効化
            m_IsInputLocked = true;
            if (m_DialogueRunner != null)
            {
                // DialogueRunnerの進行を一時停止（DialogueRunnerのAPIに応じて調整が必要な可能性あり）
                // 一般的には、DialogueRunnerのOnDialogueCompleteイベントや進行制御を使用
            }

            // 指定秒数後に待機を解除（CoroutineまたはDOTween.DelayedCallを使用）
            StartCoroutine(WaitAndUnlock(seconds));
        }

        /// <summary>
        /// 待機処理のコルーチン
        /// </summary>
        /// <param name="seconds">待機秒数</param>
        private IEnumerator WaitAndUnlock(int seconds)
        {
            yield return new WaitForSeconds(seconds);

            // タイピングインジケーターを非表示
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(false);
            }

            // 入力ロックを解除
            m_IsInputLocked = false;
        }

        /// <summary>
        /// UnlockTopicコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<UnlockTopic "TopicID">>
        /// 推論ボードに新しいトピックを追加する
        /// </summary>
        /// <param name="topicID">解放するトピックのID</param>
        private void UnlockTopicCommand(string topicID)
        {
            // ResourcesフォルダからTopicDataを読み込み
            TopicData topicData = Resources.Load<TopicData>($"Topics/{topicID}");
            
            if (topicData == null)
            {
                Debug.LogWarning($"ScenarioManager: Failed to load TopicData from Resources/Topics/{topicID}");
                return;
            }

            // 推論ボード（DeductionBoard）にトピックを追加
            // DeductionBoardは後続タスクで実装予定のため、現在はDebug.Logのみで対応
            Debug.Log($"ScenarioManager: Topic unlocked - {topicData.Title} (ID: {topicID})");
            // TODO: DeductionBoardが実装されたら、以下のように呼び出す
            // DeductionBoard.Instance.AddTopic(topicData);

            // Yarn変数を更新: $has_topic_{topicID} = true
            string variableName = $"has_topic_{topicID}";
            SetVariable<bool>(variableName, true);
        }

        /// <summary>
        /// Glitchコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<Glitch 3>>
        /// 画面にノイズ演出を適用する
        /// </summary>
        /// <param name="level">グリッチの強度レベル（1-5程度を想定）</param>
        private void GlitchCommand(int level)
        {
            // MetaEffectControllerまたは専用のGlitchEffectコンポーネントにグリッチ効果を要求
            // MetaEffectControllerが未実装の場合は、Debug.Logのみで対応（後続タスクで実装）
            Debug.Log($"ScenarioManager: Glitch command - Level: {level}");
            // TODO: MetaEffectControllerが実装されたら、以下のように呼び出す
            // MetaEffectController.Instance.PlayGlitchEffect(level);
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
            m_DialogueRunner.StartDialogue(targetNode);
        }

        /// <summary>
        /// シナリオを停止
        /// </summary>
        public void StopScenario()
        {
            if (m_DialogueRunner != null)
            {
                m_DialogueRunner.Stop();
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
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot get variable {variableName}. DialogueRunner or VariableStorage is not initialized.");
                return default(T);
            }

            // DialogueRunner.VariableStorageから変数を取得
            if (m_DialogueRunner.VariableStorage.TryGetValue(variableName, out var value))
            {
                // Yarn SpinnerのVariableStorageは通常、object型で値を返すため、キャストが必要
                if (value is T typedValue)
                {
                    return typedValue;
                }
                else
                {
                    Debug.LogWarning($"ScenarioManager: Variable {variableName} type mismatch. Expected {typeof(T)}, got {value?.GetType()}");
                }
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: Variable {variableName} not found in VariableStorage.");
            }

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
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot set variable {variableName}. DialogueRunner or VariableStorage is not initialized.");
                return;
            }

            // DialogueRunner.VariableStorageに変数を設定
            m_DialogueRunner.VariableStorage.SetValue(variableName, value);
        }
        #endregion
    }
}
