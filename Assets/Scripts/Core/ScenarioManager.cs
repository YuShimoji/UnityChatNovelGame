using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.Effects;
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
        [SerializeField] private ProjectFoundPhone.UI.ChatController m_ChatController;
        [SerializeField] private string m_StartNode = "Start";

        /// <summary>
        /// 入力ロック状態（StartWaitCommandで使用）
        /// 将来的にDialogueRunnerの進行制御で使用予定
        /// </summary>
        #pragma warning disable CS0414 // フィールドは割り当てられているが、値が使用されていない
        private bool m_IsInputLocked = false;
        #pragma warning restore CS0414

        [Header("Debug")]
        [SerializeField] private string m_DebugScenarioID;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            RegisterCustomCommands();
            
            // デバッグ用: IDが設定されていれば自動再生
            if (!string.IsNullOrEmpty(m_DebugScenarioID))
            {
                PlayScenario(m_DebugScenarioID);
            }
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
                // Unity 6の非推奨APIを新しいAPIに置き換え
                m_ChatController = FindFirstObjectByType<ProjectFoundPhone.UI.ChatController>();
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
            m_DialogueRunner.AddCommandHandler("StartWait", StartWaitCommandCoroutine);
            m_DialogueRunner.AddCommandHandler<string>("UnlockTopic", UnlockTopicCommand);
            m_DialogueRunner.AddCommandHandler<int>("Glitch", GlitchCommand);
            m_DialogueRunner.AddCommandHandler<string>("SystemMessage", SystemMessageCommand);
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
            m_DialogueRunner.RemoveCommandHandler("SystemMessage");
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
                // 画像が見つからない場合はテキストとしてフォールバック表示
                if (m_ChatController != null)
                {
                    m_ChatController.AddMessage(charID, $"[Image: {imageID}]");
                }
                return;
            }

            // ChatControllerに画像メッセージとして送信
            if (m_ChatController != null)
            {
                m_ChatController.AddImageMessage(charID, imageSprite);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. Image from {charID}: {imageID}");
            }
        }

        /// <summary>
        /// StartWaitコマンドのCoroutineハンドラ
        /// Yarnスクリプトから呼び出される: <<StartWait 15>>
        /// 指定秒数待機し、その間入力をロックする
        /// Coroutineを返すことでDialogueRunnerの進行を自動的にブロックする
        /// </summary>
        /// <param name="parameters">コマンドパラメータ（[0]: 待機秒数）</param>
        private Coroutine StartWaitCommandCoroutine(string[] parameters)
        {
            int seconds = 1;
            if (parameters.Length > 0)
            {
                int.TryParse(parameters[0], out seconds);
            }
            return StartCoroutine(StartWaitRoutine(seconds));
        }

        /// <summary>
        /// StartWaitの待機処理コルーチン
        /// DialogueRunnerはこのCoroutine完了まで次の行に進まない
        /// </summary>
        /// <param name="seconds">待機秒数</param>
        private IEnumerator StartWaitRoutine(int seconds)
        {
            // タイピングインジケーターを表示
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(true);
            }

            // 入力ロックを有効化
            m_IsInputLocked = true;

            // 指定秒数待機（DialogueRunnerの進行はCoroutine完了までブロックされる）
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
            if (DeductionBoard.Instance != null)
            {
                DeductionBoard.Instance.AddTopic(topicData);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: DeductionBoard not found. Topic unlocked - {topicData.Title} (ID: {topicID})");
            }

            // Yarn変数を更新: $has_topic_{topicID} = true
            string variableName = $"has_topic_{topicID}";
            SetVariable<bool>(variableName, true);
        }

        /// <summary>
        /// Glitchコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<Glitch 3>>
        /// 画面にノイズ演出を適用する
        /// </summary>
        /// <param name="level">グリッチの強度レベル（0-3程度を想定）</param>
        private void GlitchCommand(int level)
        {
            // MetaEffectControllerにグリッチ効果を要求
            if (MetaEffectController.Instance != null)
            {
                // Local implementation uses PlayGlitchEffect
                MetaEffectController.Instance.PlayGlitchEffect(level);
                Debug.Log($"ScenarioManager: Glitch command executed - Level: {level}");
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: MetaEffectController instance is not available. Glitch level: {level}");
            }
        }

        /// <summary>
        /// SystemMessageコマンドのハンドラ
        /// Yarnスクリプトから呼び出される: <<SystemMessage "Text">>
        /// チャット内にシステム通知（中央揃えグレーテキスト）を表示する
        /// </summary>
        /// <param name="text">表示するシステムメッセージ</param>
        private void SystemMessageCommand(string text)
        {
            if (m_ChatController != null)
            {
                m_ChatController.AddSystemMessage(text);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. System message: {text}");
            }
        }
        #endregion

        #region ScriptableObject Scenario System
        /// <summary>
        /// ID指定でシナリオをロードして再生
        /// </summary>
        /// <param name="scenarioID">Resources/ChatScenarios/以下のパス</param>
        public void PlayScenario(string scenarioID)
        {
            ChatScenarioData data = Resources.Load<ChatScenarioData>($"ChatScenarios/{scenarioID}");
            if (data != null)
            {
                PlayScenario(data);
            }
            else
            {
                Debug.LogError($"ScenarioManager: Could not find ChatScenarioData at Resources/ChatScenarios/{scenarioID}");
            }
        }

        /// <summary>
        /// ScriptableObjectベースのシナリオデータの再生を開始
        /// </summary>
        /// <param name="data">再生するシナリオデータ</param>
        public void PlayScenario(ChatScenarioData data)
        {
            if (data == null)
            {
                Debug.LogWarning("ScenarioManager: PlayScenario called with null data.");
                return;
            }

            StartCoroutine(PlayScenarioRoutine(data));
        }

        private IEnumerator PlayScenarioRoutine(ChatScenarioData data)
        {
            // 入力をロック
            m_IsInputLocked = true;

            foreach (var message in data.Messages)
            {
                // タイピング演出
                if (message.TypingDelay > 0)
                {
                    if (m_ChatController != null) m_ChatController.ShowTypingIndicator(true);
                    yield return new WaitForSeconds(message.TypingDelay);
                    if (m_ChatController != null) m_ChatController.ShowTypingIndicator(false);
                }

                // メッセージ表示
                if (m_ChatController != null)
                {
                    m_ChatController.AddMessage(message.SenderID, message.Text);
                }

                // 選択肢がある場合
                if (message.Choices != null && message.Choices.Count > 0)
                {
                    bool choiceMade = false;
                    ChatScenarioData nextScenario = null;

                    List<string> choiceTexts = new List<string>();
                    foreach (var choice in message.Choices)
                    {
                        choiceTexts.Add(choice.Text);
                    }

                    if (m_ChatController != null)
                    {
                        m_ChatController.ShowChoices(choiceTexts, (index) =>
                        {
                            // 選択された次のシナリオを取得
                            if (index >= 0 && index < message.Choices.Count)
                            {
                                nextScenario = message.Choices[index].NextScenario;
                            }
                            choiceMade = true;
                        });
                    }
                    else
                    {
                        // UIがない場合は強制進行（またはエラー）
                        Debug.LogError("ScenarioManager: ChatController missing for choices.");
                        choiceMade = true;
                    }

                    // 選択待ち
                    yield return new WaitUntil(() => choiceMade);

                    // 次のシナリオがあれば再生（現在のループは終了）
                    if (nextScenario != null)
                    {
                        // 再帰的に呼び出すのではなく、コルーチンを新しく開始して現在のコルーチンを終了
                        StartCoroutine(PlayScenarioRoutine(nextScenario));
                        yield break;
                    }
                }
            }

            // シナリオ終了時の処理（必要なら）
            m_IsInputLocked = false;

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
            // TryGetValue<T>の型引数を明示的に指定
            if (m_DialogueRunner.VariableStorage.TryGetValue<T>(variableName, out var value))
            {
                // Yarn SpinnerのVariableStorageは通常、object型で値を返すため、キャストが必要
                if (value != null)
                {
                    return value;
                }
                else
                {
                    Debug.LogWarning($"ScenarioManager: Variable {variableName} is null.");
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

            // Yarn SpinnerのVariableStorageは型ごとに異なるSetValueオーバーロードを持つ
            // string, float, bool型に対応
            if (value is string stringValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(variableName, stringValue);
            }
            else if (value is float floatValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(variableName, floatValue);
            }
            else if (value is bool boolValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(variableName, boolValue);
            }
            else
            {
                // その他の型は文字列に変換して設定
                m_DialogueRunner.VariableStorage.SetValue(variableName, value?.ToString() ?? string.Empty);
                Debug.LogWarning($"ScenarioManager: Variable {variableName} set as string (type: {typeof(T).Name})");
            }
        }
        #endregion
    }
}
