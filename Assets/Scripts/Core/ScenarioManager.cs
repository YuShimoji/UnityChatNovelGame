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
    /// Yarn Spinner縺ｮDialogueRunner繧偵Λ繝・・縺励√き繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨ｒ蜃ｦ逅・☆繧九す繝翫Μ繧ｪ邂｡逅・け繝ｩ繧ｹ
    /// 繝√Ε繝・ヨ繧ｷ繧ｹ繝・Β縺ｨ騾｣謳ｺ縺励※繝｡繝・そ繝ｼ繧ｸ陦ｨ遉ｺ繧・ヨ繝斐ャ繧ｯ隗｣謾ｾ縺ｪ縺ｩ繧貞宛蠕｡縺吶ｋ
    /// </summary>
    public class ScenarioManager : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private DialogueRunner m_DialogueRunner;
        [SerializeField] private ProjectFoundPhone.UI.ChatController m_ChatController;
        [SerializeField] private string m_StartNode = "Start";

        /// <summary>
        /// 蜈･蜉帙Ο繝・け迥ｶ諷具ｼ・tartWaitCommand縺ｧ菴ｿ逕ｨ・・        /// 蟆・擂逧・↓DialogueRunner縺ｮ騾ｲ陦悟宛蠕｡縺ｧ菴ｿ逕ｨ莠亥ｮ・        /// </summary>
        #pragma warning disable CS0414 // 繝輔ぅ繝ｼ繝ｫ繝峨・蜑ｲ繧雁ｽ薙※繧峨ｌ縺ｦ縺・ｋ縺後∝､縺御ｽｿ逕ｨ縺輔ｌ縺ｦ縺・↑縺・        private bool m_IsInputLocked = false;
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
            
            // 繝・ヰ繝・げ逕ｨ: ID縺瑚ｨｭ螳壹＆繧後※縺・ｌ縺ｰ閾ｪ蜍募・逕・            if (!string.IsNullOrEmpty(m_DebugScenarioID))
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
        /// 蠢・ｦ√↑繧ｳ繝ｳ繝昴・繝阪Φ繝医・蛻晄悄蛹・        /// </summary>
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
                // Unity 6縺ｮ髱樊耳螂ｨAPI繧呈眠縺励＞API縺ｫ鄂ｮ縺肴鋤縺・                m_ChatController = FindFirstObjectByType<ProjectFoundPhone.UI.ChatController>();
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
            if (m_DialogueRunner == null)
            {
                return;
            }

            // DialogueRunner縺ｫ繧ｫ繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ繧堤匳骭ｲ
            // Yarn Spinner縺ｮ繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ縺ｯ騾壼ｸｸ縲《tring[]驟榊・縺ｧ蠑墓焚繧貞女縺大叙繧・            m_DialogueRunner.AddCommandHandler<string, string>("Message", MessageCommand);
            m_DialogueRunner.AddCommandHandler<string, string>("Image", ImageCommand);
            m_DialogueRunner.AddCommandHandler<int>("StartWait", StartWaitCommand);
            m_DialogueRunner.AddCommandHandler<string>("UnlockTopic", UnlockTopicCommand);
            m_DialogueRunner.AddCommandHandler<int>("Glitch", GlitchCommand);
        }

        /// <summary>
        /// 繧ｫ繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨・逋ｻ骭ｲ繧定ｧ｣髯､
        /// </summary>
        private void UnregisterCustomCommands()
        {
            if (m_DialogueRunner == null)
            {
                return;
            }

            // 逋ｻ骭ｲ縺励◆繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ繧定ｧ｣髯､
            m_DialogueRunner.RemoveCommandHandler("Message");
            m_DialogueRunner.RemoveCommandHandler("Image");
            m_DialogueRunner.RemoveCommandHandler("StartWait");
            m_DialogueRunner.RemoveCommandHandler("UnlockTopic");
            m_DialogueRunner.RemoveCommandHandler("Glitch");
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
                return;
            }

            // ChatController縺ｫ逕ｻ蜒上Γ繝・そ繝ｼ繧ｸ縺ｨ縺励※騾∽ｿ｡
            // 迴ｾ蝨ｨ縺ｮAddMessage()縺ｯ繝・く繧ｹ繝医・縺ｿ蟇ｾ蠢懊・縺溘ａ縲∫判蜒終D繧貞性繧繝・く繧ｹ繝医→縺励※騾∽ｿ｡
            // 蠕檎ｶ壹ち繧ｹ繧ｯ縺ｧ逕ｻ蜒上Γ繝・そ繝ｼ繧ｸ蟆ら畑縺ｮ繝｡繧ｽ繝・ラ繧定ｿｽ蜉縺吶ｋ莠亥ｮ・            if (m_ChatController != null)
            {
                m_ChatController.AddMessage(charID, $"[Image: {imageID}]");
            }
            else
            {
                Debug.LogWarning($"ScenarioManager: ChatController not available. Image from {charID}: {imageID}");
            }
        }

        /// <summary>
        /// StartWait繧ｳ繝槭Φ繝峨・繝上Φ繝峨Λ
        /// Yarn繧ｹ繧ｯ繝ｪ繝励ヨ縺九ｉ蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧・ <<StartWait 15>>
        /// 謖・ｮ夂ｧ呈焚蠕・ｩ溘＠縲√◎縺ｮ髢灘・蜉帙ｒ繝ｭ繝・け縺吶ｋ
        /// </summary>
        /// <param name="seconds">蠕・ｩ溽ｧ呈焚</param>
        private void StartWaitCommand(int seconds)
        {
            // 繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ繧定｡ｨ遉ｺ
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(true);
            }

            // 蜈･蜉帙Ο繝・け繧呈怏蜉ｹ蛹・            m_IsInputLocked = true;
            if (m_DialogueRunner != null)
            {
                // DialogueRunner縺ｮ騾ｲ陦後ｒ荳譎ょ●豁｢・・ialogueRunner縺ｮAPI縺ｫ蠢懊§縺ｦ隱ｿ謨ｴ縺悟ｿ・ｦ√↑蜿ｯ閭ｽ諤ｧ縺ゅｊ・・                // 荳闊ｬ逧・↓縺ｯ縲．ialogueRunner縺ｮOnDialogueComplete繧､繝吶Φ繝医ｄ騾ｲ陦悟宛蠕｡繧剃ｽｿ逕ｨ
            }

            // 謖・ｮ夂ｧ呈焚蠕後↓蠕・ｩ溘ｒ隗｣髯､・・oroutine縺ｾ縺溘・DOTween.DelayedCall繧剃ｽｿ逕ｨ・・            StartCoroutine(WaitAndUnlock(seconds));
        }

        /// <summary>
        /// 蠕・ｩ溷・逅・・繧ｳ繝ｫ繝ｼ繝√Φ
        /// </summary>
        /// <param name="seconds">蠕・ｩ溽ｧ呈焚</param>
        private IEnumerator WaitAndUnlock(int seconds)
        {
            yield return new WaitForSeconds(seconds);

            // 繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ繧帝撼陦ｨ遉ｺ
            if (m_ChatController != null)
            {
                m_ChatController.ShowTypingIndicator(false);
            }

            // 蜈･蜉帙Ο繝・け繧定ｧ｣髯､
            m_IsInputLocked = false;
        }

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
            string variableName = $"has_topic_{topicID}";
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
            // MetaEffectController縺ｫ繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ隕∵ｱ・            if (MetaEffectController.Instance != null)
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
        /// ScriptableObject繝吶・繧ｹ縺ｮ繧ｷ繝翫Μ繧ｪ繝・・繧ｿ縺ｮ蜀咲函繧帝幕蟋・        /// </summary>
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
            m_IsInputLocked = true;

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

                // 驕ｸ謚櫁い縺後≠繧句ｴ蜷・                if (message.Choices != null && message.Choices.Count > 0)
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
                            // 驕ｸ謚槭＆繧後◆谺｡縺ｮ繧ｷ繝翫Μ繧ｪ繧貞叙蠕・                            if (index >= 0 && index < message.Choices.Count)
                            {
                                nextScenario = message.Choices[index].NextScenario;
                            }
                            choiceMade = true;
                        });
                    }
                    else
                    {
                        // UI縺後↑縺・ｴ蜷医・蠑ｷ蛻ｶ騾ｲ陦鯉ｼ医∪縺溘・繧ｨ繝ｩ繝ｼ・・                        Debug.LogError("ScenarioManager: ChatController missing for choices.");
                        choiceMade = true;
                    }

                    // 驕ｸ謚槫ｾ・■
                    yield return new WaitUntil(() => choiceMade);

                    // 谺｡縺ｮ繧ｷ繝翫Μ繧ｪ縺後≠繧後・蜀咲函・育樟蝨ｨ縺ｮ繝ｫ繝ｼ繝励・邨ゆｺ・ｼ・                    if (nextScenario != null)
                    {
                        // 蜀榊ｸｰ逧・↓蜻ｼ縺ｳ蜃ｺ縺吶・縺ｧ縺ｯ縺ｪ縺上√さ繝ｫ繝ｼ繝√Φ繧呈眠縺励￥髢句ｧ九＠縺ｦ迴ｾ蝨ｨ縺ｮ繧ｳ繝ｫ繝ｼ繝√Φ繧堤ｵゆｺ・                        StartCoroutine(PlayScenarioRoutine(nextScenario));
                        yield break;
                    }
                }
            }

            // 繧ｷ繝翫Μ繧ｪ邨ゆｺ・凾縺ｮ蜃ｦ逅・ｼ亥ｿ・ｦ√↑繧会ｼ・            m_IsInputLocked = false;

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 繧ｷ繝翫Μ繧ｪ繧帝幕蟋・        /// </summary>
        /// <param name="nodeName">髢句ｧ九☆繧戯arn繝弱・繝牙錐・育怐逡･譎ゅ・m_StartNode繧剃ｽｿ逕ｨ・・/param>
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
        /// 繧ｷ繝翫Μ繧ｪ繧貞●豁｢
        /// </summary>
        public void StopScenario()
        {
            if (m_DialogueRunner != null)
            {
                m_DialogueRunner.Stop();
            }
        }

        /// <summary>
        /// Yarn螟画焚縺ｮ蛟､繧貞叙蠕・        /// </summary>
        /// <typeparam name="T">螟画焚縺ｮ蝙・/typeparam>
        /// <param name="variableName">螟画焚蜷・/param>
        /// <returns>螟画焚縺ｮ蛟､</returns>
        public T GetVariable<T>(string variableName)
        {
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot get variable {variableName}. DialogueRunner or VariableStorage is not initialized.");
                return default(T);
            }

            // DialogueRunner.VariableStorage縺九ｉ螟画焚繧貞叙蠕・            // TryGetValue<T>縺ｮ蝙句ｼ墓焚繧呈・遉ｺ逧・↓謖・ｮ・            if (m_DialogueRunner.VariableStorage.TryGetValue<T>(variableName, out var value))
            {
                // Yarn Spinner縺ｮVariableStorage縺ｯ騾壼ｸｸ縲｛bject蝙九〒蛟､繧定ｿ斐☆縺溘ａ縲√く繝｣繧ｹ繝医′蠢・ｦ・                if (value != null)
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
        /// Yarn螟画焚縺ｮ蛟､繧定ｨｭ螳・        /// </summary>
        /// <typeparam name="T">螟画焚縺ｮ蝙・/typeparam>
        /// <param name="variableName">螟画焚蜷・/param>
        /// <param name="value">險ｭ螳壹☆繧句､</param>
        public void SetVariable<T>(string variableName, T value)
        {
            if (m_DialogueRunner == null || m_DialogueRunner.VariableStorage == null)
            {
                Debug.LogWarning($"ScenarioManager: Cannot set variable {variableName}. DialogueRunner or VariableStorage is not initialized.");
                return;
            }

            // Yarn Spinner縺ｮVariableStorage縺ｯ蝙九＃縺ｨ縺ｫ逡ｰ縺ｪ繧鬼etValue繧ｪ繝ｼ繝舌・繝ｭ繝ｼ繝峨ｒ謖√▽
            // string, float, bool蝙九↓蟇ｾ蠢・            if (value is string stringValue)
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
                // 縺昴・莉悶・蝙九・譁・ｭ怜・縺ｫ螟画鋤縺励※險ｭ螳・                m_DialogueRunner.VariableStorage.SetValue(variableName, value?.ToString() ?? string.Empty);
                Debug.LogWarning($"ScenarioManager: Variable {variableName} set as string (type: {typeof(T).Name})");
            }
        }
        #endregion
    }
}
