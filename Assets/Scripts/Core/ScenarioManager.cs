using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
#if YARN_SPINNER
using Yarn.Unity;
#endif
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.Effects;
using DG.Tweening;

namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// Yarn SpinnerのDialogueRunnerをラップし、カスタムコマンドを処理するシナリオ管理クラス
    /// チャットシステムと連携してメッセージ表示やトピック解錠などを制御する
    /// </summary>
    public class ScenarioManager : MonoBehaviour
    {
        private const string DefaultYarnProjectPath = "Assets/Resources/Yarn/Project.yarnproject";
        #region Private Fields
#if YARN_SPINNER
        [SerializeField] private DialogueRunner m_DialogueRunner;
        private ChatDialogueView m_DialogueView;
#endif
        [SerializeField] private ProjectFoundPhone.UI.ChatController m_ChatController;
        [SerializeField] private string m_StartNode = "Start";

        private bool m_IsInputLocked = false;
        private CancellationTokenSource m_WaitCancellation;
        private bool m_IsWaiting = false;
        private BranchThreadState m_BranchThreadState = new BranchThreadState();
        private ChannelData m_CurrentChannel;

        /// <summary>宣言済みサブスレッド (threadId -> SubthreadData)</summary>
        private readonly Dictionary<string, SubthreadData> m_DeclaredThreads
            = new Dictionary<string, SubthreadData>();

        /// <summary>サブスレッドが宣言されたときのイベント (threadId, displayName)</summary>
        public event Action<string, string> OnThreadDeclared;

        /// <summary>サブスレッドにメッセージが追加されたときのイベント (threadId)</summary>
        public event Action<string> OnThreadMessageAdded;

        [Header("Auto Start")]
        [SerializeField] private bool m_AutoStartYarn = false;

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
            
            // デバッグ用: IDが設定されていれば SOベースシナリオを自動再生
            if (!string.IsNullOrEmpty(m_DebugScenarioID))
            {
                PlayScenario(m_DebugScenarioID);
            }
#if YARN_SPINNER
            // Yarn シナリオの自動開始（Inspector で有効化）
            else if (m_AutoStartYarn && m_DialogueRunner != null)
            {
                StartScenario(m_StartNode);
            }
#endif
        }

        private void OnDestroy()
        {
            UnregisterCustomCommands();
            // Play停止時: CancelActiveWait の Cancel() が Yarn の非同期継続を発火し、
            // 既に node が無い DialogueRunner.Continue() で DialogueException が出るのを防止。
            // ダイアログを先に停止してから Wait をキャンセルする。
            if (m_DialogueRunner != null && m_DialogueRunner.IsDialogueRunning)
            {
                m_DialogueRunner.Stop();
            }
            CancelActiveWait();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 必要なコンポーネントの初期化
        /// </summary>
        private void InitializeComponents()
        {
#if YARN_SPINNER
            if (m_DialogueRunner == null)
            {
                m_DialogueRunner = GetComponent<DialogueRunner>();
            }

            if (m_DialogueRunner == null)
            {
                Debug.LogError("ScenarioManager: DialogueRunner component is required!");
            }

            m_DialogueView = FindFirstObjectByType<ChatDialogueView>();

            // ChatDialogueView が DialogueRunner の Presenter リストに未登録なら追加
            if (m_DialogueRunner != null && m_DialogueView != null)
            {
                bool alreadyRegistered = false;
                foreach (var p in m_DialogueRunner.DialoguePresenters)
                {
                    if (p == m_DialogueView)
                    {
                        alreadyRegistered = true;
                        break;
                    }
                }
                if (!alreadyRegistered)
                {
                    var presenters = new System.Collections.Generic.List<DialoguePresenterBase>(m_DialogueRunner.DialoguePresenters);
                    presenters.Add(m_DialogueView);
                    m_DialogueRunner.DialoguePresenters = presenters;
                    Debug.Log("ScenarioManager: ChatDialogueView をランタイムで DialogueRunner に登録しました");
                }
            }
#endif

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
#if YARN_SPINNER
            if (m_DialogueRunner == null)
            {
                return;
            }

            // DialogueRunnerにカスタムコマンドハンドラを登録
            // Yarn Spinnerのコマンドハンドラは通常、string[]配列で引数を受け取る
            m_DialogueRunner.AddCommandHandler<string, string>("Message", MessageCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("Image", ImageCommand);
            m_DialogueRunner.AddCommandHandler<float>("StartWait", StartWaitCommand);
            m_DialogueRunner.AddCommandHandler("SkipWait", SkipWaitCommand);
            m_DialogueRunner.AddCommandHandler<string>("UnlockTopic", UnlockTopicCommand);
            m_DialogueRunner.AddCommandHandler<int>("Glitch", GlitchCommand);
            m_DialogueRunner.AddCommandHandler<string>("SystemMessage", SystemMessageCommand);
            m_DialogueRunner.AddCommandHandler<string, string, string>("MessageTagged", MessageTaggedCommand);
            m_DialogueRunner.AddCommandHandler<string>("Typing", TypingCommand);
            m_DialogueRunner.AddCommandHandler<int>("EndDay", EndDayCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("DeclareThread", DeclareThreadCommand);
            m_DialogueRunner.AddCommandHandler<string, string, string>("DeclareThreadTyped", DeclareThreadTypedCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("AddThreadMessage", AddThreadMessageCommand);
            m_DialogueRunner.AddCommandHandler<string, string, string>("AddThreadChat", AddThreadChatCommand);
            m_DialogueRunner.AddCommandHandler<int>("AddHalluciCoin", AddHalluciCoinCommand);
            m_DialogueRunner.AddCommandHandler<string>("CompleteThread", CompleteThreadCommand);
            m_DialogueRunner.AddCommandHandler<string, string, string>("DeclareThreadLatent", DeclareThreadLatentCommand);
            m_DialogueRunner.AddCommandHandler<string>("ManifestThread", ManifestThreadCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("BeginBranch", BeginBranchCommand);
            m_DialogueRunner.AddCommandHandler<bool>("EndBranch", EndBranchCommand);
            m_DialogueRunner.AddCommandHandler<string>("SetBranchReflection", SetBranchReflectionCommand);
#endif
        }

        /// <summary>
        /// カスタムコマンドの登録を解除
        /// </summary>
        private void UnregisterCustomCommands()
        {
#if YARN_SPINNER
            if (m_DialogueRunner == null)
            {
                return;
            }

            // 登録したコマンドハンドラを解除
            m_DialogueRunner.RemoveCommandHandler("Message");
            m_DialogueRunner.RemoveCommandHandler("Image");
            m_DialogueRunner.RemoveCommandHandler("StartWait");
            m_DialogueRunner.RemoveCommandHandler("SkipWait");
            m_DialogueRunner.RemoveCommandHandler("UnlockTopic");
            m_DialogueRunner.RemoveCommandHandler("Glitch");
            m_DialogueRunner.RemoveCommandHandler("SystemMessage");
            m_DialogueRunner.RemoveCommandHandler("MessageTagged");
            m_DialogueRunner.RemoveCommandHandler("Typing");
            m_DialogueRunner.RemoveCommandHandler("EndDay");
            m_DialogueRunner.RemoveCommandHandler("DeclareThread");
            m_DialogueRunner.RemoveCommandHandler("DeclareThreadTyped");
            m_DialogueRunner.RemoveCommandHandler("AddThreadMessage");
            m_DialogueRunner.RemoveCommandHandler("AddThreadChat");
            m_DialogueRunner.RemoveCommandHandler("AddHalluciCoin");
            m_DialogueRunner.RemoveCommandHandler("CompleteThread");
            m_DialogueRunner.RemoveCommandHandler("DeclareThreadLatent");
            m_DialogueRunner.RemoveCommandHandler("ManifestThread");
            m_DialogueRunner.RemoveCommandHandler("BeginBranch");
            m_DialogueRunner.RemoveCommandHandler("EndBranch");
            m_DialogueRunner.RemoveCommandHandler("SetBranchReflection");
#endif
        }
        #endregion

        #region Custom Command Handlers
        /// <summary>
        /// Messageコマンドのハンドラ
        /// Yarnスクリプトから呼び出される <<Message "CharID" "Text">>
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
        /// LineTag 付きメッセージコマンド
        /// Yarn: &lt;&lt;MessageTagged "CharID" "Text" "lineTag"&gt;&gt;
        /// </summary>
        private void MessageTaggedCommand(string charID, string text, string lineTag)
        {
            if (m_ChatController != null)
            {
                m_ChatController.AddMessage(charID, text, lineTag);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. MessageTagged from {charID}: {text} [tag:{lineTag}]");
            }
        }

        /// <summary>
        /// Imageコマンドのハンドラ
        /// Yarnスクリプトから呼び出される <<Image "CharID" "ImageID">>
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

#if YARN_SPINNER
        /// <summary>
        /// StartWaitコマンドのハンドラ
        /// Yarnスクリプトから呼び出される <<StartWait 2>>
        /// YarnTaskを返すことでDialogueRunnerの進行を自動的にブロックする
        /// </summary>
        /// <param name="seconds">蠕・ｩ溽ｧ呈焚</param>
        private async YarnTask StartWaitCommand(float seconds)
        {
            // 早送りモード時は最小遅延のみ
            if (m_DialogueView != null && m_DialogueView.FastForwardEnabled)
            {
                await YarnTask.Delay(30);
                return;
            }

            CancelActiveWait();
            m_IsWaiting = true;
            m_WaitCancellation = new CancellationTokenSource();

            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(true);
            }

            SetInputLocked(true);

            await YarnTask.Delay((int)(seconds * 1000), m_WaitCancellation.Token).SuppressCancellationThrow();

            // ダイアログが停止された場合、CancelActiveWait が既にクリーンアップ済み。
            // 最小限の片付けのみ行い return する。
            if (m_DialogueRunner == null || !m_DialogueRunner.IsDialogueRunning)
            {
                m_IsWaiting = false;
                if (m_WaitCancellation != null)
                {
                    m_WaitCancellation.Dispose();
                    m_WaitCancellation = null;
                }
                return;
            }

            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(false);
            }

            SetInputLocked(false);

            m_IsWaiting = false;
            if (m_WaitCancellation != null)
            {
                m_WaitCancellation.Dispose();
                m_WaitCancellation = null;
            }
        }

        private void SkipWaitCommand()
        {
            CancelActiveWait();
        }
#endif

        /// <summary>
        /// UnlockTopicコマンドのハンドラ
        /// Yarnスクリプトから呼び出される <<UnlockTopic "TopicID">>
        /// 推論ボードに新しいトピックを追加する
        /// </summary>
        /// <param name="topicID">解錠するトピックのID</param>
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
                // DeductionBoard は凍結中（D-1 決定）。警告ではなく通常ログに留める
                Debug.Log($"ScenarioManager: Topic unlocked - {topicData.Title} (ID: {topicID})");
            }

            // Yarn変数を更新: $has_topic_{topicID} = true
            string variableName = $"$has_topic_{topicID}";
            SetVariable<bool>(variableName, true);

            // 分岐内でトピック解錠された場合、TransferFlags に自動登録
            if (m_BranchThreadState != null && m_BranchThreadState.IsActive)
            {
                AddBranchTransferFlag(topicID);
                Debug.Log($"ScenarioManager: Topic '{topicID}' auto-tracked in branch '{m_BranchThreadState.ActiveBranchId}'");
            }
        }

        /// <summary>
        /// Glitchコマンドのハンドラ
        /// Yarnスクリプトから呼び出される <<Glitch 3>>
        /// 画面にノイズ演出を適用する
        /// </summary>
        /// <param name="level">グリッチの強度レベル（1-3程度を想定）</param>
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
        /// Yarnスクリプトから呼び出される <<SystemMessage "Text">>
        /// チャット内にシステム通知（中央揃え・グレーテキスト）を表示する
        /// </summary>
        /// <param name="text">表示するシステムメッセージ</param>
        private void SystemMessageCommand(string text)
        {
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(false);
                m_ChatController.AddSystemMessage(text);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. System message: {text}");
            }
        }

        private void TypingCommand(string showStr)
        {
            bool show = showStr.Equals("true", System.StringComparison.OrdinalIgnoreCase);
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(show);
            }
        }

        /// <summary>
        /// 現在再生中のチャンネルを設定する。
        /// DashboardController.OnChannelClicked から呼び出される。
        /// EndDayCommand が正しいチャンネルにDay進捗を記録するために必要。
        /// </summary>
        public void SetCurrentChannel(ChannelData channel)
        {
            m_CurrentChannel = channel;
        }

        /// <summary>
        /// EndDay コマンドハンドラ: Day終了の共通処理
        /// Yarn: <<EndDay N>> → "--- N日目 終了 ---" システムメッセージ + Day進捗記録
        /// マルチDayチャプターでは最終Day完了時のみチャンネルを完了にする。
        /// </summary>
        private void EndDayCommand(int dayNumber)
        {
            SystemMessageCommand($"--- {dayNumber}日目 終了 ---");

            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
            {
                var saveData = SaveManager.Instance.CurrentSaveData;

                if (m_CurrentChannel != null)
                {
                    string channelId = m_CurrentChannel.ChannelID;

                    // Day進捗を記録
                    saveData.ChannelDayProgress[channelId] = dayNumber;
                    Debug.Log($"ScenarioManager: EndDay {dayNumber} — channel '{channelId}' day progress recorded.");

                    // 最終Dayならチャンネル完了
                    if (dayNumber >= m_CurrentChannel.TotalDays)
                    {
                        if (!saveData.CompletedChannelIDs.Contains(channelId))
                        {
                            saveData.CompletedChannelIDs.Add(channelId);
                            Debug.Log($"ScenarioManager: Channel '{channelId}' marked completed (final day).");
                        }
                    }
                }
                else
                {
                    // フォールバック: CurrentChannel未設定時は旧挙動
                    string channelId = $"ch{dayNumber}";
                    if (!saveData.CompletedChannelIDs.Contains(channelId))
                    {
                        saveData.CompletedChannelIDs.Add(channelId);
                        Debug.Log($"ScenarioManager: EndDay {dayNumber} (fallback) — channel '{channelId}' marked completed.");
                    }
                }

                // EndDay時にオートセーブ（重要イベントのためクールダウンを無視）
                SaveManager.Instance.AutoSave(forceSave: true);
            }
        }

        /// <summary>
        /// サブスレッドを宣言し、メインチャットに通知を表示する（2引数版、type=Annotation）。
        /// Yarn: &lt;&lt;DeclareThread "threadId" "displayName"&gt;&gt;
        /// </summary>
        private void DeclareThreadCommand(string threadId, string displayName)
        {
            DeclareThreadInternal(threadId, displayName, ThreadType.Annotation);
        }

        /// <summary>
        /// サブスレッドを宣言（3引数版、type指定）。
        /// Yarn: &lt;&lt;DeclareThreadTyped "threadId" "type" "displayName"&gt;&gt;
        /// type: "A"=Annotation, "B"=Tracking, "C"=Scout, "branch"=Branch
        /// </summary>
        private void DeclareThreadTypedCommand(string threadId, string typeStr, string displayName)
        {
            DeclareThreadInternal(threadId, displayName, ParseThreadType(typeStr));
        }

        private void DeclareThreadInternal(string threadId, string displayName, ThreadType type, bool latent = false)
        {
            if (m_DeclaredThreads.ContainsKey(threadId))
            {
                Debug.LogWarning($"ScenarioManager: Thread '{threadId}' already declared, skipping.");
                return;
            }

            var thread = new SubthreadData
            {
                ThreadId = threadId,
                DisplayName = displayName,
                Type = type,
                IsLatent = latent
            };
            m_DeclaredThreads[threadId] = thread;

            if (latent)
            {
                // 潜在登録: UIに通知せず、データのみ登録
                Debug.Log($"ScenarioManager: Thread declared (latent) — id='{threadId}', type={type}, name='{displayName}'");
                return;
            }

            // メインチャットに出現通知（型アイコン+型色付き）
            if (m_ChatController != null)
            {
                string icon = GetTypeIcon(type);
                string colorHex = GetTypeColorHex(type);
                m_ChatController.AddSystemMessage(
                    $"<color=#{colorHex}>{icon}</color> 新しいスレッド「{displayName}」が利用可能です");
            }

            OnThreadDeclared?.Invoke(threadId, displayName);
            Debug.Log($"ScenarioManager: Thread declared — id='{threadId}', type={type}, name='{displayName}'");
        }

        private static ThreadType ParseThreadType(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return ThreadType.Annotation;
            switch (typeStr.Trim().ToLowerInvariant())
            {
                case "a":
                case "annotation":
                    return ThreadType.Annotation;
                case "b":
                case "tracking":
                    return ThreadType.Tracking;
                case "c":
                case "scout":
                    return ThreadType.Scout;
                case "branch":
                    return ThreadType.Branch;
                default:
                    Debug.LogWarning($"ScenarioManager: Unknown thread type '{typeStr}', defaulting to Annotation.");
                    return ThreadType.Annotation;
            }
        }

        /// <summary>
        /// Yarn: &lt;&lt;CompleteThread "threadId"&gt;&gt;
        /// スレッドを完了状態にする。サイドバーでグレーアウト+チェックマーク表示。
        /// </summary>
        private void CompleteThreadCommand(string threadId)
        {
            if (!m_DeclaredThreads.TryGetValue(threadId, out var thread))
            {
                Debug.LogWarning($"ScenarioManager: Thread '{threadId}' not found.");
                return;
            }

            thread.IsCompleted = true;

            if (m_ChatController != null)
            {
                string icon = GetTypeIcon(thread.Type);
                string colorHex = GetTypeColorHex(thread.Type);
                m_ChatController.AddSystemMessage(
                    $"<color=#{colorHex}>{icon}</color> スレッド「{thread.DisplayName}」が完了しました");
            }

            OnThreadCompleted?.Invoke(threadId);
            Debug.Log($"ScenarioManager: Thread completed — id='{threadId}'");
        }

        /// <summary>スレッド完了イベント</summary>
        public event Action<string> OnThreadCompleted;

        /// <summary>
        /// Yarn: &lt;&lt;AddHalluciCoin amount&gt;&gt;
        /// HalluciCoin を静かに加算する。通知なし（ダッシュボード復帰時にバッジパルスで気づく）。
        /// </summary>
        private void AddHalluciCoinCommand(int amount)
        {
            var cm = FindFirstObjectByType<ContradictionManager>();
            if (cm != null)
            {
                cm.AddCoinSilent(amount);
                SyncHalluciCoinVariable(cm.HalluciCoin);
                Debug.Log($"ScenarioManager: AddHalluciCoin — +{amount} (silent), total={cm.HalluciCoin}");
            }
            else
            {
                Debug.LogWarning("ScenarioManager: ContradictionManager not found. Cannot add HalluciCoin.");
            }
        }

        /// <summary>HalluciCoin の値を Yarn 変数 $halluci_coin に同期する</summary>
        private void SyncHalluciCoinVariable(int value)
        {
            SetVariable<float>("$halluci_coin", (float)value);
        }

        /// <summary>
        /// Yarn: &lt;&lt;DeclareThreadLatent "threadId" "type" "displayName"&gt;&gt;
        /// スレッドを潜在登録する。UIに表示せず、ManifestThread で顕在化するまで見えない。
        /// </summary>
        private void DeclareThreadLatentCommand(string threadId, string typeStr, string displayName)
        {
            DeclareThreadInternal(threadId, displayName, ParseThreadType(typeStr), latent: true);
        }

        /// <summary>
        /// Yarn: &lt;&lt;ManifestThread "threadId"&gt;&gt;
        /// 潜在スレッドを顕在化する。通知メッセージ+サイドバー追加を発火する。
        /// </summary>
        private void ManifestThreadCommand(string threadId)
        {
            if (!m_DeclaredThreads.TryGetValue(threadId, out var thread))
            {
                Debug.LogWarning($"ScenarioManager: Thread '{threadId}' not found. DeclareThreadLatent first.");
                return;
            }

            if (!thread.IsLatent)
            {
                Debug.LogWarning($"ScenarioManager: Thread '{threadId}' is already visible.");
                return;
            }

            thread.IsLatent = false;

            // 通知メッセージ
            if (m_ChatController != null)
            {
                string icon = GetTypeIcon(thread.Type);
                string colorHex = GetTypeColorHex(thread.Type);
                m_ChatController.AddSystemMessage(
                    $"<color=#{colorHex}>{icon}</color> 新しいスレッド「{thread.DisplayName}」が利用可能です");
            }

            // UI更新 (サイドバーにエントリ追加)
            OnThreadDeclared?.Invoke(threadId, thread.DisplayName);

            Debug.Log($"ScenarioManager: Thread manifested — id='{threadId}', type={thread.Type}, name='{thread.DisplayName}'");
        }

        /// <summary>ThreadType の表示アイコンを返す</summary>
        private static string GetTypeIcon(ThreadType type)
        {
            switch (type)
            {
                case ThreadType.Annotation: return "[A]";
                case ThreadType.Tracking: return "[B]";
                case ThreadType.Scout: return "[C]";
                case ThreadType.Branch: return "[>]";
                default: return "[?]";
            }
        }

        /// <summary>ThreadType の色を16進数文字列で返す (リッチテキスト用)</summary>
        private static string GetTypeColorHex(ThreadType type)
        {
            switch (type)
            {
                case ThreadType.Annotation: return "4A90D9";
                case ThreadType.Tracking: return "4CAF50";
                case ThreadType.Scout: return "FF9800";
                case ThreadType.Branch: return "9C27B0";
                default: return "4A90D9";
            }
        }

        /// <summary>
        /// サブスレッドにメッセージを追加する（チャット画面には表示しない）。
        /// Yarn: &lt;&lt;AddThreadMessage "threadId" "text"&gt;&gt;
        /// </summary>
        private void AddThreadMessageCommand(string threadId, string text)
        {
            if (!m_DeclaredThreads.TryGetValue(threadId, out var thread))
            {
                Debug.LogWarning($"ScenarioManager: Thread '{threadId}' not found. Declare it first.");
                return;
            }

            thread.ChatHistory.Add(new SavedChatMessage
            {
                Type = ChatMessageType.System,
                CharacterID = null,
                Text = text
            });
            thread.UnreadCount++;
            OnThreadMessageAdded?.Invoke(threadId);
        }

        /// <summary>
        /// サブスレッドにキャラクター付きメッセージを追加する。
        /// Yarn: &lt;&lt;AddThreadChat "threadId" "charID" "text"&gt;&gt;
        /// </summary>
        private void AddThreadChatCommand(string threadId, string charID, string text)
        {
            if (!m_DeclaredThreads.TryGetValue(threadId, out var thread))
            {
                Debug.LogWarning($"ScenarioManager: Thread '{threadId}' not found. Declare it first.");
                return;
            }

            thread.ChatHistory.Add(new SavedChatMessage
            {
                Type = ChatMessageType.Normal,
                CharacterID = charID,
                Text = text
            });
            thread.UnreadCount++;
            OnThreadMessageAdded?.Invoke(threadId);
        }

        /// <summary>宣言済みスレッドを取得</summary>
        public SubthreadData GetDeclaredThread(string threadId)
        {
            m_DeclaredThreads.TryGetValue(threadId, out var thread);
            return thread;
        }

        /// <summary>宣言済みの全スレッドを返す</summary>
        public IReadOnlyDictionary<string, SubthreadData> GetAllDeclaredThreads()
        {
            return m_DeclaredThreads;
        }

        /// <summary>ロード時にサブスレッドを再登録する</summary>
        public void RegisterDeclaredThread(SubthreadData thread)
        {
            if (thread == null || string.IsNullOrEmpty(thread.ThreadId)) return;
            m_DeclaredThreads[thread.ThreadId] = thread;
            OnThreadDeclared?.Invoke(thread.ThreadId, thread.DisplayName);
        }

        /// <summary>宣言済みスレッドをクリア（シナリオ開始時）</summary>
        public void ClearDeclaredThreads()
        {
            m_DeclaredThreads.Clear();
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
            SetInputLocked(true);

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

                    // 驕ｸ謚槫ｾ・■
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

            // シナリオ終了時の処理
            SetInputLocked(false);

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 入力がロックされているかどうか
        /// </summary>
        /// <summary>
        /// Returns a safe snapshot of branch-thread state.
        /// </summary>
        public BranchThreadState GetBranchThreadStateSnapshot()
        {
            return m_BranchThreadState?.Clone() ?? new BranchThreadState();
        }

        /// <summary>
        /// Applies branch-thread state on load.
        /// </summary>
        public void ApplyBranchThreadState(BranchThreadState state)
        {
            m_BranchThreadState = state?.Clone() ?? new BranchThreadState();
        }

        /// <summary>
        /// Starts branch-thread tracking.
        /// </summary>
        public void BeginBranchThread(string branchId)
        {
            m_BranchThreadState ??= new BranchThreadState();
            m_BranchThreadState.ActiveBranchId = branchId;
            m_BranchThreadState.IsActive = true;
            m_BranchThreadState.WasCompleted = false;
            m_BranchThreadState.TransferFlags.Clear();
        }

        /// <summary>
        /// Adds one transfer flag from branch to main thread.
        /// </summary>
        public void AddBranchTransferFlag(string flagId)
        {
            if (string.IsNullOrWhiteSpace(flagId))
            {
                return;
            }

            m_BranchThreadState ??= new BranchThreadState();
            if (!m_BranchThreadState.TransferFlags.Contains(flagId))
            {
                m_BranchThreadState.TransferFlags.Add(flagId);
            }
        }

        /// <summary>
        /// Ends branch-thread tracking.
        /// </summary>
        public void EndBranchThread(bool completed)
        {
            m_BranchThreadState ??= new BranchThreadState();
            m_BranchThreadState.IsActive = false;
            m_BranchThreadState.WasCompleted = completed;
        }

        /// <summary>
        /// Yarn: &lt;&lt;BeginBranch "branchId" "displayName"&gt;&gt;
        /// 分岐スレッドを宣言し、自動的にスレッド切替を行う。
        /// 以降のメッセージは分岐スレッドに流れる。
        /// </summary>
        private void BeginBranchCommand(string branchId, string displayName)
        {
            // 分岐スレッドを宣言 (未宣言の場合)
            if (!m_DeclaredThreads.ContainsKey(branchId))
            {
                DeclareThreadInternal(branchId, displayName, ThreadType.Branch);
            }

            // ブランチ状態を開始
            BeginBranchThread(branchId);

            // ChatController を分岐スレッドに自動切替
            if (m_ChatController != null)
            {
                var thread = GetDeclaredThread(branchId);
                m_ChatController.SetActiveThreadType(ThreadType.Branch);
                m_ChatController.SwitchToThread(branchId, thread?.ChatHistory);
            }

            // ThreadSwitcherController のヘッダーバー更新
            var threadSwitcher = FindFirstObjectByType<ThreadSwitcherController>();
            threadSwitcher?.ForceUpdateHeaderBar(branchId);

            Debug.Log($"ScenarioManager: Branch started — id='{branchId}', name='{displayName}'");
        }

        /// <summary>
        /// Yarn: &lt;&lt;EndBranch true|false&gt;&gt;
        /// 分岐スレッドを終了し、メインスレッドに自動復帰する。
        /// ReflectionMessage が設定されていればメインスレッドに反映メッセージを投入する。
        /// </summary>
        private void EndBranchCommand(bool completed)
        {
            string branchId = m_BranchThreadState?.ActiveBranchId;

            // 反映メッセージを決定: Yarn指定 > 自動生成 > なし
            string reflectionMessage = ResolveReflectionMessage();

            // 反映メッセージをメインスレッドに投入
            if (m_ChatController != null && !string.IsNullOrEmpty(reflectionMessage))
            {
                // 一時的にメインスレッドに切替えてシステムメッセージを追加
                string currentThread = m_ChatController.ActiveThreadId;
                m_ChatController.SetActiveThreadType(null);
                m_ChatController.SwitchToThread(null);

                string colorHex = GetTypeColorHex(ThreadType.Branch);
                m_ChatController.AddSystemMessage(
                    $"<color=#{colorHex}>[>]</color> {reflectionMessage}");
            }
            else if (m_ChatController != null)
            {
                // 反映メッセージなし: そのままメインに復帰
                m_ChatController.SetActiveThreadType(null);
                m_ChatController.SwitchToThread(null);
            }

            // ブランチ状態を終了
            EndBranchThread(completed);

            // ThreadSwitcherController のヘッダーバー更新 (メイン表示)
            var threadSwitcher = FindFirstObjectByType<ThreadSwitcherController>();
            threadSwitcher?.ForceUpdateHeaderBar(null);

            Debug.Log($"ScenarioManager: Branch ended — id='{branchId}', completed={completed}");
        }

        /// <summary>
        /// Yarn: &lt;&lt;SetBranchReflection "text"&gt;&gt;
        /// EndBranch 時にメインスレッドへ投入する反映メッセージを設定する。
        /// </summary>
        private void SetBranchReflectionCommand(string text)
        {
            m_BranchThreadState ??= new BranchThreadState();
            m_BranchThreadState.ReflectionMessage = text;
        }

        /// <summary>
        /// EndBranch 時の反映メッセージを決定する。
        /// 優先順位: Yarn指定 (SetBranchReflection) > TransferFlags自動生成 > null
        /// </summary>
        private string ResolveReflectionMessage()
        {
            if (m_BranchThreadState == null) return null;

            // Yarn作者が明示指定した場合はそのまま使用
            if (!string.IsNullOrEmpty(m_BranchThreadState.ReflectionMessage))
            {
                return m_BranchThreadState.ReflectionMessage;
            }

            // TransferFlags (分岐内で解錠されたトピック) から自動生成
            if (m_BranchThreadState.TransferFlags != null && m_BranchThreadState.TransferFlags.Count > 0)
            {
                var topicNames = new List<string>();
                foreach (var topicId in m_BranchThreadState.TransferFlags)
                {
                    TopicData topicData = Resources.Load<TopicData>($"Topics/{topicId}");
                    topicNames.Add(topicData != null ? topicData.Title : topicId);
                }
                return $"分岐で新情報を確認: {string.Join(", ", topicNames)}";
            }

            return null;
        }

        public bool IsInputLocked => m_IsInputLocked;

        /// <summary>
        /// 入力ロック状態を設定し、ChatControllerの入力欄を連動制御する
        /// </summary>
        private void SetInputLocked(bool locked)
        {
            m_IsInputLocked = locked;
            if (m_ChatController != null)
            {
                m_ChatController.SetInputEnabled(!locked);
            }
        }

        /// <summary>
        /// シナリオを開始
        /// </summary>
        /// <param name="nodeName">開始するYarnノード名（省略時はm_StartNodeを使用）</param>
        public void StartScenario(string nodeName = null)
        {
#if YARN_SPINNER
            string targetNode = nodeName ?? m_StartNode;
            if (!ValidateDialogueStart(targetNode))
            {
                return;
            }

            m_DialogueRunner.StartDialogue(targetNode);
#else
            Debug.LogWarning("ScenarioManager: Yarn Spinner is not available. StartScenario is a no-op.");
#endif
        }

        /// <summary>
        /// シナリオを停止
        /// </summary>
        public void StopScenario()
        {
            // CancelActiveWait を先に呼び、非同期コマンドハンドラ（StartWaitCommand 等）の
            // 継続を正常完了させる。Stop() は 1 フレーム遅延して呼ぶことで、
            // Continue()/SetSelectedOption() が空 VM に対して発火する競合を防ぐ。
            CancelActiveWait();
#if YARN_SPINNER
            if (m_DialogueRunner != null && m_DialogueRunner.IsDialogueRunning)
            {
                StartCoroutine(StopDialogueDeferred());
            }
#endif
        }

#if YARN_SPINNER
        private IEnumerator StopDialogueDeferred()
        {
            yield return null;
            if (m_DialogueRunner != null && m_DialogueRunner.IsDialogueRunning)
            {
                m_DialogueRunner.Stop();
            }
        }
#endif

        /// <summary>
        /// Yarn変数の値を取得
        /// </summary>
        /// <typeparam name="T">変数の型</typeparam>
        /// <param name="variableName">螟画焚蜷・/param>
        /// <returns>変数の値</returns>
        public T GetVariable<T>(string variableName)
        {
#if YARN_SPINNER
            string normalizedName = NormalizeVariableName(variableName);
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot get variable {normalizedName}. DialogueRunner or VariableStorage is not initialized.");
                return default(T);
            }

            // DialogueRunner.VariableStorageから変数を取得
            // TryGetValue<T>の型安全チェック
            if (m_DialogueRunner.VariableStorage.TryGetValue<T>(normalizedName, out var value))
            {
                if (value != null)
                {
                    return value;
                }
                else
                {
                    Debug.LogWarning($"ScenarioManager: Variable {normalizedName} is null.");
                }
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: Variable {normalizedName} not found in VariableStorage.");
            }
#endif
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
#if YARN_SPINNER
            string normalizedName = NormalizeVariableName(variableName);
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot set variable {normalizedName}. DialogueRunner or VariableStorage is not initialized.");
                return;
            }

            // string, float, boolの順で型判定
            if (value is string stringValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, stringValue);
            }
            else if (value is float floatValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, floatValue);
            }
            else if (value is bool boolValue)
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, boolValue);
            }
            else
            {
                m_DialogueRunner.VariableStorage.SetValue(normalizedName, value?.ToString() ?? string.Empty);
                Debug.LogWarning($"ScenarioManager: Variable {normalizedName} set as string (type: {typeof(T).Name})");
            }
#endif
        }

        private void CancelActiveWait()
        {
            if (m_WaitCancellation != null && !m_WaitCancellation.IsCancellationRequested)
            {
                m_WaitCancellation.Cancel();
            }

            if (m_IsWaiting)
            {
                if (m_ChatController != null)
                {
                    m_ChatController.ShowTypingIndicator(false);
                }
                SetInputLocked(false);
                m_IsWaiting = false;
            }
        }

        private string NormalizeVariableName(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                return variableName;
            }

            return variableName.StartsWith("$") ? variableName : "$" + variableName;
        }

#if YARN_SPINNER
        private bool ValidateDialogueStart(string targetNode)
        {
            if (m_DialogueRunner == null)
            {
                LogBrokenYarnFile(DefaultYarnProjectPath, "dialogue_runner_missing");
                Debug.LogError("ScenarioManager: DialogueRunner is not initialized!");
                return false;
            }

            YarnProject yarnProject = m_DialogueRunner.YarnProject;
            if (yarnProject == null)
            {
                LogBrokenYarnFile(DefaultYarnProjectPath, "yarn_project_missing");
                Debug.LogError("ScenarioManager: YarnProject is not assigned.");
                return false;
            }

            if (yarnProject.Program == null || yarnProject.NodeNames == null || yarnProject.NodeNames.Length == 0)
            {
                LogBrokenYarnFile(DefaultYarnProjectPath, "program_invalid");
                Debug.LogError("ScenarioManager: YarnProject does not contain a compiled program.");
                return false;
            }

            foreach (string nodeName in yarnProject.NodeNames)
            {
                if (nodeName == targetNode)
                {
                    return true;
                }
            }

            LogBrokenYarnFile(ResolveLikelyBrokenYarnFile(targetNode), $"missing_node:{targetNode}");
            Debug.LogError($"ScenarioManager: Target node '{targetNode}' was not found in YarnProject.");
            return false;
        }

        private void LogBrokenYarnFile(string assetPath, string reason)
        {
            Debug.LogError($"ContentAuthoring: broken-yarn-file={assetPath} reason={reason}");
        }

        private string ResolveLikelyBrokenYarnFile(string targetNode)
        {
            string yarnDirectoryPath = Path.Combine(Application.dataPath, "Resources", "Yarn");
            if (!Directory.Exists(yarnDirectoryPath))
            {
                return DefaultYarnProjectPath;
            }

            string[] yarnFiles = Directory.GetFiles(yarnDirectoryPath, "*.yarn", SearchOption.AllDirectories);
            foreach (string fullPath in yarnFiles)
            {
                try
                {
                    string fileText = File.ReadAllText(fullPath);
                    if (fileText.Contains($"title: {targetNode}", System.StringComparison.Ordinal))
                    {
                        return ToAssetPath(fullPath);
                    }
                }
                catch (IOException)
                {
                }
                catch (System.UnauthorizedAccessException)
                {
                }
            }

            return DefaultYarnProjectPath;
        }

        private string ToAssetPath(string fullPath)
        {
            string normalizedFullPath = fullPath.Replace('\\', '/');
            string normalizedAssetsPath = Application.dataPath.Replace('\\', '/');

            if (normalizedFullPath.StartsWith(normalizedAssetsPath, System.StringComparison.OrdinalIgnoreCase))
            {
                return "Assets" + normalizedFullPath.Substring(normalizedAssetsPath.Length);
            }

            return normalizedFullPath;
        }
#endif

        #endregion
    }
}

