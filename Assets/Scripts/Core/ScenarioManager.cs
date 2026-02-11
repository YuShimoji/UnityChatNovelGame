using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    /// Yarn Spinner縺ｮDialogueRunner繧偵Λ繝・・縺励√き繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨ｒ蜃ｦ逅・☆繧九す繝翫Μ繧ｪ邂｡逅・け繝ｩ繧ｹ
    /// 繝√Ε繝・ヨ繧ｷ繧ｹ繝・Β縺ｨ騾｣謳ｺ縺励※繝｡繝・そ繝ｼ繧ｸ陦ｨ遉ｺ繧・ヨ繝斐ャ繧ｯ隗｣謾ｾ縺ｪ縺ｩ繧貞宛蠕｡縺吶ｋ
    /// </summary>
    public class ScenarioManager : MonoBehaviour
    {
        #region Private Fields
#if YARN_SPINNER
        [SerializeField] private DialogueRunner m_DialogueRunner;
#endif
        [SerializeField] private ProjectFoundPhone.UI.ChatController m_ChatController;
        [SerializeField] private string m_StartNode = "Start";

        private bool m_IsInputLocked = false;
        private CancellationTokenSource m_WaitCancellation;
        private bool m_IsWaiting = false;

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
            
            // 繝・ヰ繝・げ逕ｨ: ID縺瑚ｨｭ螳壹＆繧後※縺・ｌ縺ｰ SO 繝吶・繧ｹ繧ｷ繝翫Μ繧ｪ繧定・蜍募・逕・
            if (!string.IsNullOrEmpty(m_DebugScenarioID))
            {
                PlayScenario(m_DebugScenarioID);
            }
#if YARN_SPINNER
            // Yarn 繧ｷ繝翫Μ繧ｪ縺ｮ閾ｪ蜍暮幕蟋具ｼ・nspector 縺ｧ譛牙柑蛹厄ｼ・
            else if (m_AutoStartYarn && m_DialogueRunner != null)
            {
                StartScenario(m_StartNode);
            }
#endif
        }

        private void OnDestroy()
        {
            UnregisterCustomCommands();
            CancelActiveWait();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 蠢・ｦ√↑繧ｳ繝ｳ繝昴・繝阪Φ繝医・蛻晄悄蛹・
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
#endif

            if (m_ChatController == null)
            {
                // Unity 6縺ｮ髱樊耳螂ｨAPI繧呈眠縺励＞API縺ｫ鄂ｮ縺肴鋤縺・
                m_ChatController = FindFirstObjectByType<ProjectFoundPhone.UI.ChatController>();
            }

            if (m_ChatController == null)
            {
                Debug.LogWarning("ScenarioManager: ChatController not found. Some features may not work.");
            }
        }

        /// <summary>
        /// Yarn Spinner縺ｮ繧ｫ繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨ｒ逋ｻ骭ｲ
        /// </summary>
        private void RegisterCustomCommands()
        {
#if YARN_SPINNER
            if (m_DialogueRunner == null)
            {
                return;
            }

            // DialogueRunner縺ｫ繧ｫ繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ繧堤匳骭ｲ
            // Yarn Spinner縺ｮ繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ縺ｯ騾壼ｸｸ縲《tring[]驟榊・縺ｧ蠑墓焚繧貞女縺大叙繧・
            m_DialogueRunner.AddCommandHandler<string, string>("Message", MessageCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("Image", ImageCommand);
            m_DialogueRunner.AddCommandHandler<float>("StartWait", StartWaitCommand);
            m_DialogueRunner.AddCommandHandler("SkipWait", SkipWaitCommand);
            m_DialogueRunner.AddCommandHandler<string>("UnlockTopic", UnlockTopicCommand);
            m_DialogueRunner.AddCommandHandler<int>("Glitch", GlitchCommand);
            m_DialogueRunner.AddCommandHandler<string>("SystemMessage", SystemMessageCommand);
#endif
        }

        /// <summary>
        /// 繧ｫ繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨・逋ｻ骭ｲ繧定ｧ｣髯､
        /// </summary>
        private void UnregisterCustomCommands()
        {
#if YARN_SPINNER
            if (m_DialogueRunner == null)
            {
                return;
            }

            // 逋ｻ骭ｲ縺励◆繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ繧定ｧ｣髯､
            m_DialogueRunner.RemoveCommandHandler("Message");
            m_DialogueRunner.RemoveCommandHandler("Image");
            m_DialogueRunner.RemoveCommandHandler("StartWait");
            m_DialogueRunner.RemoveCommandHandler("SkipWait");
            m_DialogueRunner.RemoveCommandHandler("UnlockTopic");
            m_DialogueRunner.RemoveCommandHandler("Glitch");
            m_DialogueRunner.RemoveCommandHandler("SystemMessage");
#endif
        }
        #endregion

        #region Custom Command Handlers
        /// <summary>
        /// Message繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<Message "CharID" "Text">>
        /// </summary>
        /// <param name="charID">繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼID</param>
        /// <param name="text">繝｡繝・そ繝ｼ繧ｸ繝・く繧ｹ繝・/param>
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
        /// Image繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<Image "CharID" "ImageID">>
        /// </summary>
        /// <param name="charID">繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼID</param>
        /// <param name="imageID">逕ｻ蜒上Μ繧ｽ繝ｼ繧ｹ縺ｮID</param>
        private void ImageCommand(string charID, string imageID)
        {
            // Resources繝輔か繝ｫ繝縺九ｉ逕ｻ蜒上ｒ隱ｭ縺ｿ霎ｼ縺ｿ
            Sprite imageSprite = Resources.Load<Sprite>($"Images/{imageID}");
            
            if (imageSprite == null)
            {
                Debug.LogWarning($"ScenarioManager: Failed to load image from Resources/Images/{imageID}");
                // 逕ｻ蜒上′隕九▽縺九ｉ縺ｪ縺・ｴ蜷医・繝・く繧ｹ繝医→縺励※繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ陦ｨ遉ｺ
                if (m_ChatController != null)
                {
                    m_ChatController.AddMessage(charID, $"[Image: {imageID}]");
                }
                return;
            }

            // ChatController縺ｫ逕ｻ蜒上Γ繝・そ繝ｼ繧ｸ縺ｨ縺励※騾∽ｿ｡
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
        /// StartWait繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<StartWait 2>>
        /// YarnTask繧定ｿ斐☆縺薙→縺ｧDialogueRunner縺ｮ騾ｲ陦後ｒ閾ｪ蜍慕噪縺ｫ繝悶Ο繝・け縺吶ｋ
        /// </summary>
        /// <param name="seconds">蠕・ｩ溽ｧ呈焚</param>
        private async YarnTask StartWaitCommand(float seconds)
        {
            CancelActiveWait();
            m_IsWaiting = true;
            m_WaitCancellation = new CancellationTokenSource();

            // 繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ繧定｡ｨ遉ｺ
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(true);
            }

            // 蜈･蜉帙Ο繝・け繧呈怏蜉ｹ蛹・
            SetInputLocked(true);

            // 謖・ｮ夂ｧ呈焚蠕・ｩ滂ｼ・ialogueRunner縺ｮ騾ｲ陦後・YarnTask螳御ｺ・∪縺ｧ繝悶Ο繝・け縺輔ｌ繧具ｼ・
            await YarnTask.Delay((int)(seconds * 1000), m_WaitCancellation.Token).SuppressCancellationThrow();

            // 繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ繧帝撼陦ｨ遉ｺ
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(false);
            }

            // 蜈･蜉帙Ο繝・け繧定ｧ｣髯､
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
        /// UnlockTopic繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<UnlockTopic "TopicID">>
        /// 謗ｨ隲悶・繝ｼ繝峨↓譁ｰ縺励＞繝医ヴ繝・け繧定ｿｽ蜉縺吶ｋ
        /// </summary>
        /// <param name="topicID">隗｣謾ｾ縺吶ｋ繝医ヴ繝・け縺ｮID</param>
        private void UnlockTopicCommand(string topicID)
        {
            // Resources繝輔か繝ｫ繝縺九ｉTopicData繧定ｪｭ縺ｿ霎ｼ縺ｿ
            TopicData topicData = Resources.Load<TopicData>($"Topics/{topicID}");
            
            if (topicData == null)
            {
                Debug.LogWarning($"ScenarioManager: Failed to load TopicData from Resources/Topics/{topicID}");
                return;
            }

            // 謗ｨ隲悶・繝ｼ繝会ｼ・eductionBoard・峨↓繝医ヴ繝・け繧定ｿｽ蜉
            if (DeductionBoard.Instance != null)
            {
                DeductionBoard.Instance.AddTopic(topicData);
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: DeductionBoard not found. Topic unlocked - {topicData.Title} (ID: {topicID})");
            }

            // Yarn螟画焚繧呈峩譁ｰ: $has_topic_{topicID} = true
            string variableName = $"$has_topic_{topicID}";
            SetVariable<bool>(variableName, true);
        }

        /// <summary>
        /// Glitch繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<Glitch 3>>
        /// 逕ｻ髱｢縺ｫ繝弱う繧ｺ貍泌・繧帝←逕ｨ縺吶ｋ
        /// </summary>
        /// <param name="level">繧ｰ繝ｪ繝・メ縺ｮ蠑ｷ蠎ｦ繝ｬ繝吶Ν・・-3遞句ｺｦ繧呈Φ螳夲ｼ・/param>
        private void GlitchCommand(int level)
        {
            // MetaEffectController縺ｫ繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ隕∵ｱ・
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
        /// SystemMessage繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<SystemMessage "Text">>
        /// 繝√Ε繝・ヨ蜀・↓繧ｷ繧ｹ繝・Β騾夂衍・井ｸｭ螟ｮ謠・∴繧ｰ繝ｬ繝ｼ繝・く繧ｹ繝茨ｼ峨ｒ陦ｨ遉ｺ縺吶ｋ
        /// </summary>
        /// <param name="text">陦ｨ遉ｺ縺吶ｋ繧ｷ繧ｹ繝・Β繝｡繝・そ繝ｼ繧ｸ</param>
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
        /// ID謖・ｮ壹〒繧ｷ繝翫Μ繧ｪ繧偵Ο繝ｼ繝峨＠縺ｦ蜀咲函
        /// </summary>
        /// <param name="scenarioID">Resources/ChatScenarios/莉･荳九・繝代せ</param>
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
        /// ScriptableObject繝吶・繧ｹ縺ｮ繧ｷ繝翫Μ繧ｪ繝・・繧ｿ縺ｮ蜀咲函繧帝幕蟋・
        /// </summary>
        /// <param name="data">蜀咲函縺吶ｋ繧ｷ繝翫Μ繧ｪ繝・・繧ｿ</param>
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
            // 蜈･蜉帙ｒ繝ｭ繝・け
            SetInputLocked(true);

            foreach (var message in data.Messages)
            {
                // 繧ｿ繧､繝斐Φ繧ｰ貍泌・
                if (message.TypingDelay > 0)
                {
                    if (m_ChatController != null) m_ChatController.ShowTypingIndicator(true);
                    yield return new WaitForSeconds(message.TypingDelay);
                    if (m_ChatController != null) m_ChatController.ShowTypingIndicator(false);
                }

                // 繝｡繝・そ繝ｼ繧ｸ陦ｨ遉ｺ
                if (m_ChatController != null)
                {
                    m_ChatController.AddMessage(message.SenderID, message.Text);
                }

                // 驕ｸ謚櫁い縺後≠繧句ｴ蜷・
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
                            // 驕ｸ謚槭＆繧後◆谺｡縺ｮ繧ｷ繝翫Μ繧ｪ繧貞叙蠕・
                            if (index >= 0 && index < message.Choices.Count)
                            {
                                nextScenario = message.Choices[index].NextScenario;
                            }
                            choiceMade = true;
                        });
                    }
                    else
                    {
                        // UI縺後↑縺・ｴ蜷医・蠑ｷ蛻ｶ騾ｲ陦鯉ｼ医∪縺溘・繧ｨ繝ｩ繝ｼ・・
                        Debug.LogError("ScenarioManager: ChatController missing for choices.");
                        choiceMade = true;
                    }

                    // 驕ｸ謚槫ｾ・■
                    yield return new WaitUntil(() => choiceMade);

                    // 谺｡縺ｮ繧ｷ繝翫Μ繧ｪ縺後≠繧後・蜀咲函・育樟蝨ｨ縺ｮ繝ｫ繝ｼ繝励・邨ゆｺ・ｼ・
                    if (nextScenario != null)
                    {
                        // 蜀榊ｸｰ逧・↓蜻ｼ縺ｳ蜃ｺ縺吶・縺ｧ縺ｯ縺ｪ縺上√さ繝ｫ繝ｼ繝√Φ繧呈眠縺励￥髢句ｧ九＠縺ｦ迴ｾ蝨ｨ縺ｮ繧ｳ繝ｫ繝ｼ繝√Φ繧堤ｵゆｺ・
                        StartCoroutine(PlayScenarioRoutine(nextScenario));
                        yield break;
                    }
                }
            }

            // 繧ｷ繝翫Μ繧ｪ邨ゆｺ・凾縺ｮ蜃ｦ逅・
            SetInputLocked(false);

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 蜈･蜉帙′繝ｭ繝・け縺輔ｌ縺ｦ縺・ｋ縺九←縺・°
        /// </summary>
        public bool IsInputLocked => m_IsInputLocked;

        /// <summary>
        /// 蜈･蜉帙Ο繝・け迥ｶ諷九ｒ險ｭ螳壹＠縲，hatController縺ｮ蜈･蜉帶ｬ・ｒ騾｣蜍募宛蠕｡縺吶ｋ
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
        /// 繧ｷ繝翫Μ繧ｪ繧帝幕蟋・
        /// </summary>
        /// <param name="nodeName">髢句ｧ九☆繧戯arn繝弱・繝牙錐・育怐逡･譎ゅ・m_StartNode繧剃ｽｿ逕ｨ・・/param>
        public void StartScenario(string nodeName = null)
        {
#if YARN_SPINNER
            if (m_DialogueRunner == null)
            {
                Debug.LogError("ScenarioManager: DialogueRunner is not initialized!");
                return;
            }

            string targetNode = nodeName ?? m_StartNode;
            m_DialogueRunner.StartDialogue(targetNode);
#else
            Debug.LogWarning("ScenarioManager: Yarn Spinner is not available. StartScenario is a no-op.");
#endif
        }

        /// <summary>
        /// 繧ｷ繝翫Μ繧ｪ繧貞●豁｢
        /// </summary>
        public void StopScenario()
        {
#if YARN_SPINNER
            if (m_DialogueRunner != null)
            {
                m_DialogueRunner.Stop();
            }
#endif
            CancelActiveWait();
        }

        /// <summary>
        /// Yarn螟画焚縺ｮ蛟､繧貞叙蠕・
        /// </summary>
        /// <typeparam name="T">螟画焚縺ｮ蝙・/typeparam>
        /// <param name="variableName">螟画焚蜷・/param>
        /// <returns>螟画焚縺ｮ蛟､</returns>
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

        #endregion
    }
}

