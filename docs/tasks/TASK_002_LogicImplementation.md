# Task: ChatController & ScenarioManager 繝ｭ繧ｸ繝・け螳溯｣・Status: DONE
Tier: 2
Branch: main
Owner: Worker
Created: 2026-01-06T08:15:00Z
Report: docs/reports/REPORT_TASK_002_LogicImplementation.md 

## Objective
ChatController.cs 縺ｨ ScenarioManager.cs 縺ｮTODO繧ｳ繝｡繝ｳ繝医↓險倩ｼ峨＆繧後◆繝ｭ繧ｸ繝・け繧貞ｮ溯｣・☆繧九ゅせ繧ｱ繝ｫ繝医Φ繧ｳ繝ｼ繝峨〒螳夂ｾｩ縺輔ｌ縺溘Γ繧ｽ繝・ラ縺ｮ螳溯｣・ｒ螳御ｺ・＆縺帙∝渕譛ｬ逧・↑蜍穂ｽ懊ｒ蜿ｯ閭ｽ縺ｫ縺吶ｋ縲・
螳溯｣・ｯｾ雎｡・・1. **ChatController.cs**: 繝｡繝・そ繝ｼ繧ｸ陦ｨ遉ｺ縲√せ繧ｯ繝ｭ繝ｼ繝ｫ蛻ｶ蠕｡縲√ち繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ蛻ｶ蠕｡縺ｮ繝ｭ繧ｸ繝・け
2. **ScenarioManager.cs**: Yarn Spinner騾｣謳ｺ縲√き繧ｹ繧ｿ繝繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ縺ｮ螳溯｣・
## Context
- 蜑阪ち繧ｹ繧ｯ・・ASK_001・峨〒繧ｹ繧ｱ繝ｫ繝医Φ繧ｳ繝ｼ繝峨′菴懈・貂医∩
- ChatController.cs 縺ｨ ScenarioManager.cs 縺ｫ螟壽焚縺ｮTODO繧ｳ繝｡繝ｳ繝医′谿九▲縺ｦ縺・ｋ
- 蠢・医ヱ繝・こ繝ｼ繧ｸ: Yarn Spinner, DOTween Pro, TextMeshPro・域里縺ｫ蜑肴署・・- 蜿ら・繝峨く繝･繝｡繝ｳ繝・ `譛蛻昴・繝励Ο繝ｳ繝励ヨ`・医・繝ｭ繧ｸ繧ｧ繧ｯ繝医Ν繝ｼ繝茨ｼ峨～docs/inbox/REPORT_TASK_001_UnityCoreSystemSkeleton.md`

## Focus Area
- `Assets/Scripts/UI/ChatController.cs` 縺ｮTODO螳溯｣・- `Assets/Scripts/Core/ScenarioManager.cs` 縺ｮTODO螳溯｣・- Unity C# 繧ｳ繝ｼ繝・ぅ繝ｳ繧ｰ隕冗ｴ・ｼ・ascalCase, camelCase, #region菴ｿ逕ｨ・・- SOLID蜴溷援縺ｫ蝓ｺ縺･縺剰ｨｭ險医・邯ｭ謖・- DOTween Pro 繧剃ｽｿ逕ｨ縺励◆繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ螳溯｣・- Yarn Spinner API 縺ｮ驕ｩ蛻・↑菴ｿ逕ｨ

## Forbidden Area
- 譌｢蟄倥ヵ繧｡繧､繝ｫ縺ｮ蜑企勁繝ｻ遐ｴ螢顔噪螟画峩・・ODO螳溯｣・・縺ｿ・・- Unity繝励Ο繧ｸ繧ｧ繧ｯ繝郁ｨｭ螳壹・螟画峩
- 繝代ャ繧ｱ繝ｼ繧ｸ縺ｮ霑ｽ蜉・・arn Spinner, DOTween, TextMeshPro縺ｯ譌｢縺ｫ蜑肴署・・- Prefab繧Тcene縺ｮ菴懈・・亥ｾ檎ｶ壹ち繧ｹ繧ｯ縺ｸ蛻・屬・・- 繝・せ繝医さ繝ｼ繝峨・菴懈・・亥ｾ檎ｶ壹ち繧ｹ繧ｯ縺ｸ蛻・屬・・- 螳悟・縺ｪ繧ｨ繝ｩ繝ｼ繝上Φ繝峨Μ繝ｳ繧ｰ・井ｸｻ隕√ヱ繧ｹ縺ｮ縺ｿ螳溯｣・ｼ・
## Constraints
- 繝・せ繝・ 荳ｻ隕√ヱ繧ｹ縺ｮ縺ｿ・育ｶｲ鄒・ユ繧ｹ繝医・蠕檎ｶ壹ち繧ｹ繧ｯ縺ｸ蛻・屬・・- 繝輔か繝ｼ繝ｫ繝舌ャ繧ｯ: 譁ｰ隕剰ｿｽ蜉遖∵ｭ｢・域里蟄倥ヵ繧｡繧､繝ｫ縺ｮTODO螳溯｣・・縺ｿ・・- 繧ｳ繝ｼ繝峨せ繧ｿ繧､繝ｫ: Unity C# 繝吶せ繝医・繝ｩ繧ｯ繝・ぅ繧ｹ縺ｫ蠕薙≧
- 蜻ｽ蜷崎ｦ丞援: 螟画焚蜷阪・ m_VariableName, 螳壽焚縺ｯ c_ConstantName, 髱咏噪縺ｯ s_StaticName
- Yarn Spinner API: 譛譁ｰ縺ｮ繝峨く繝･繝｡繝ｳ繝医ｒ蜿ら・・医ヰ繝ｼ繧ｸ繝ｧ繝ｳ萓晏ｭ倥・蜿ｯ閭ｽ諤ｧ縺ゅｊ・・
## DoD
- [x] ChatController.cs 縺ｮ蜈ｨTODO繧ｳ繝｡繝ｳ繝医′螳溯｣・＆繧後※縺・ｋ
  - [x] InitializeComponents() 縺ｮnull繝√ぉ繝・け縺ｨ隴ｦ蜻・  - [x] CheckUserScrollInput() 縺ｮ繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ菴咲ｽｮ逶｣隕・  - [x] CreateMessageBubble() 縺ｮPrefab繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ蛹悶→繝ｬ繧､繧｢繧ｦ繝郁ｨｭ螳・  - [x] AddMessage() 縺ｮ繝｡繝・そ繝ｼ繧ｸ霑ｽ蜉繝ｭ繧ｸ繝・け
  - [x] ShowTypingIndicator() 縺ｮAutoScroll騾｣謳ｺ
  - [x] AutoScroll() 縺ｮ繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・・OTween菴ｿ逕ｨ・・  - [x] ClearMessages() 縺ｮ螻･豁ｴ繧ｯ繝ｪ繧｢蜃ｦ逅・- [x] ScenarioManager.cs 縺ｮ蜈ｨTODO繧ｳ繝｡繝ｳ繝医′螳溯｣・＆繧後※縺・ｋ
  - [x] RegisterCustomCommands() 縺ｮ繧ｳ繝槭Φ繝臥匳骭ｲ・・arn Spinner API菴ｿ逕ｨ・・  - [x] UnregisterCustomCommands() 縺ｮ繧ｳ繝槭Φ繝芽ｧ｣髯､
  - [x] ImageCommand() 縺ｮ逕ｻ蜒剰ｪｭ縺ｿ霎ｼ縺ｿ縺ｨ騾∽ｿ｡
  - [x] StartWaitCommand() 縺ｮ蠕・ｩ溷・逅・→蜈･蜉帙Ο繝・け
  - [x] UnlockTopicCommand() 縺ｮ繝医ヴ繝・け隗｣謾ｾ縺ｨYarn螟画焚譖ｴ譁ｰ
  - [x] GlitchCommand() 縺ｮ繧ｰ繝ｪ繝・メ貍泌・・・etaEffectController騾｣謳ｺ縺ｯ蠕檎ｶ壹ち繧ｹ繧ｯ・・  - [x] StartScenario() 縺ｮDialogueRunner襍ｷ蜍・  - [x] StopScenario() 縺ｮDialogueRunner蛛懈ｭ｢
  - [x] GetVariable/SetVariable() 縺ｮVariableStorage謫堺ｽ・- [x] 蜈ｨ縺ｦ縺ｮ螳溯｣・′SOLID蜴溷援縺ｫ蝓ｺ縺･縺・※縺・ｋ
- [x] 荳ｻ隕√ヱ繧ｹ縺ｮ蜍穂ｽ懃｢ｺ隱阪′螳御ｺ・＠縺ｦ縺・ｋ
- [x] docs/inbox/ 縺ｫ繝ｬ繝昴・繝茨ｼ・EPORT_TASK_002_LogicImplementation.md・峨′菴懈・縺輔ｌ縺ｦ縺・ｋ
- [x] 譛ｬ繝√こ繝・ヨ縺ｮ Report 谺・↓繝ｬ繝昴・繝医ヱ繧ｹ縺瑚ｿｽ險倥＆繧後※縺・ｋ

## 螳溯｣・ｩｳ邏ｰ

### ChatController.cs 縺ｮ螳溯｣・・岼

#### 1. InitializeComponents()
- m_MessageBubblePrefab縲［_TypingIndicator縺ｮnull繝√ぉ繝・け
- null縺ｮ蝣ｴ蜷医・隴ｦ蜻翫Ο繧ｰ蜃ｺ蜉・
#### 2. CheckUserScrollInput()
- ScrollRect.verticalNormalizedPosition繧堤屮隕・- 繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ菴咲ｽｮ縺御ｸ九°繧我ｸ螳壻ｻ･荳企屬繧後※縺・ｋ蝣ｴ蜷医［_IsUserScrolling = true
- 髢ｾ蛟､: m_AutoScrollThreshold・医ョ繝輔か繝ｫ繝・.1・・
#### 3. CreateMessageBubble(string charID, string text)
- m_MessageBubblePrefab縺九ｉ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ繧堤函謌・- charID縺ｫ蠢懊§縺ｦ蜿ｳ蟇・○/蟾ｦ蟇・○繧定ｨｭ螳夲ｼ・player"縺ｮ蝣ｴ蜷医・蜿ｳ蟇・○縲√◎繧御ｻ･螟悶・蟾ｦ蟇・○・・- TextMeshPro繧ｳ繝ｳ繝昴・繝阪Φ繝医↓text繧定ｨｭ螳・- ContentSizeFitter縺ｧ鬮倥＆繧定・蜍戊ｪｿ謨ｴ
- m_ScrollRect.content縺ｮ蟄舌→縺励※霑ｽ蜉

#### 4. AddMessage(string charID, string text)
- CreateMessageBubble()縺ｧ繝｡繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ繧堤函謌・- m_ScrollRect.content縺ｮ蟄舌→縺励※霑ｽ蜉
- m_IsUserScrolling縺掲alse縺ｮ蝣ｴ蜷医・縺ｿAutoScroll()繧貞ｮ溯｡・
#### 5. ShowTypingIndicator(bool show)
- 陦ｨ遉ｺ譎ゅ・AutoScroll()繧貞ｮ溯｡後＠縺ｦ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ縺瑚ｦ九∴繧九ｈ縺・↓縺吶ｋ

#### 6. AutoScroll()
- ScrollRect.verticalNormalizedPosition繧・.0縺ｫ險ｭ螳・- DOTween繧剃ｽｿ逕ｨ縺励◆繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ繧｢繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ・・.3遘堤ｨ句ｺｦ・・- 繧ｹ繧ｯ繝ｭ繝ｼ繝ｫ螳御ｺ・ｾ後↓m_LastScrollPosition繧呈峩譁ｰ
- m_IsUserScrolling縺荊rue縺ｮ蝣ｴ蜷医・螳溯｡後＠縺ｪ縺・
#### 7. ClearMessages()
- m_ScrollRect.content縺ｮ蟄舌が繝悶ず繧ｧ繧ｯ繝茨ｼ医Γ繝・そ繝ｼ繧ｸ繝舌ヶ繝ｫ・峨ｒ蜈ｨ縺ｦ蜑企勁
- Destroy()繧剃ｽｿ逕ｨ

### ScenarioManager.cs 縺ｮ螳溯｣・・岼

#### 1. RegisterCustomCommands()
- DialogueRunner.AddCommandHandler()繧剃ｽｿ逕ｨ縺励※繧ｳ繝槭Φ繝峨ｒ逋ｻ骭ｲ
- 逋ｻ骭ｲ縺吶ｋ繧ｳ繝槭Φ繝・
  - "Message": MessageCommand(string, string)
  - "Image": ImageCommand(string, string)
  - "StartWait": StartWaitCommand(int)
  - "UnlockTopic": UnlockTopicCommand(string)
  - "Glitch": GlitchCommand(int)
- Yarn Spinner縺ｮAPI縺ｯ繝舌・繧ｸ繝ｧ繝ｳ萓晏ｭ倥・蜿ｯ閭ｽ諤ｧ縺後≠繧九◆繧√∵怙譁ｰ繝峨く繝･繝｡繝ｳ繝医ｒ蜿ら・

#### 2. UnregisterCustomCommands()
- 逋ｻ骭ｲ縺励◆繧ｳ繝槭Φ繝峨ワ繝ｳ繝峨Λ繧定ｧ｣髯､
- DialogueRunner.RemoveCommandHandler()繧剃ｽｿ逕ｨ

#### 3. ImageCommand(string charID, string imageID)
- Resources.Load<Sprite>($"Images/{imageID}")縺ｧ逕ｻ蜒上ｒ隱ｭ縺ｿ霎ｼ縺ｿ
- ChatController縺ｫ逕ｻ蜒上Γ繝・そ繝ｼ繧ｸ縺ｨ縺励※騾∽ｿ｡・・ddMessage()縺ｮ諡｡蠑ｵ縺ｾ縺溘・譁ｰ隕上Γ繧ｽ繝・ラ・・- 隱ｭ縺ｿ霎ｼ縺ｿ螟ｱ謨玲凾縺ｯ隴ｦ蜻翫Ο繧ｰ繧貞・蜉・
#### 4. StartWaitCommand(int seconds)
- m_ChatController.ShowTypingIndicator(true)縺ｧ繧ｿ繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ繧定｡ｨ遉ｺ
- 蜈･蜉帙Ο繝・け繧呈怏蜉ｹ蛹厄ｼ・ialogueRunner縺ｮ蜈･蜉帷┌蜉ｹ蛹悶∪縺溘・蟆ら畑繝輔Λ繧ｰ・・- Coroutine縺ｾ縺溘・DOTween.DelayedCall()縺ｧ謖・ｮ夂ｧ呈焚蠕・ｩ・- 蠕・ｩ溯ｧ｣髯､蠕後√ち繧､繝斐Φ繧ｰ繧､繝ｳ繧ｸ繧ｱ繝ｼ繧ｿ繝ｼ繧帝撼陦ｨ遉ｺ・・howTypingIndicator(false)・・
#### 5. UnlockTopicCommand(string topicID)
- Resources.Load<TopicData>($"Topics/{topicID}")縺ｧTopicData繧定ｪｭ縺ｿ霎ｼ縺ｿ
- 謗ｨ隲悶・繝ｼ繝会ｼ・eductionBoard・峨↓繝医ヴ繝・け繧定ｿｽ蜉・・eductionBoard縺ｯ蠕檎ｶ壹ち繧ｹ繧ｯ縺ｧ螳溯｣・ｺ亥ｮ夲ｼ・- Yarn螟画焚繧呈峩譁ｰ: SetVariable($"has_topic_{topicID}", true)
- 隱ｭ縺ｿ霎ｼ縺ｿ螟ｱ謨玲凾縺ｯ隴ｦ蜻翫Ο繧ｰ繧貞・蜉・
#### 6. GlitchCommand(int level)
- MetaEffectController縺ｾ縺溘・蟆ら畑縺ｮGlitchEffect繧ｳ繝ｳ繝昴・繝阪Φ繝医↓繧ｰ繝ｪ繝・メ蜉ｹ譫懊ｒ隕∵ｱ・- 繝ｬ繝吶Ν縺ｫ蠢懊§縺溷ｼｷ蠎ｦ縺ｧ繝弱う繧ｺ繧定｡ｨ遉ｺ
- MetaEffectController縺梧悴螳溯｣・・蝣ｴ蜷医・縲．ebug.Log縺ｮ縺ｿ縺ｧ蟇ｾ蠢懶ｼ亥ｾ檎ｶ壹ち繧ｹ繧ｯ縺ｧ螳溯｣・ｼ・
#### 7. StartScenario(string nodeName)
- DialogueRunner.StartDialogue(nodeName)繧貞他縺ｳ蜃ｺ縺・- nodeName縺系ull縺ｮ蝣ｴ蜷医・m_StartNode繧剃ｽｿ逕ｨ

#### 8. StopScenario()
- DialogueRunner.Stop()繧貞他縺ｳ蜃ｺ縺・
#### 9. GetVariable<T>(string variableName)
- DialogueRunner.VariableStorage縺九ｉ螟画焚繧貞叙蠕・- VariableStorage.GetValue()繧剃ｽｿ逕ｨ

#### 10. SetVariable<T>(string variableName, T value)
- DialogueRunner.VariableStorage縺ｫ螟画焚繧定ｨｭ螳・- VariableStorage.SetValue()繧剃ｽｿ逕ｨ

## Notes
- Status 縺ｯ OPEN / IN_PROGRESS / BLOCKED / DONE 繧呈Φ螳・- BLOCKED 縺ｮ蝣ｴ蜷医・縲∽ｺ句ｮ・譬ｹ諡/谺｡謇具ｼ亥呵｣懶ｼ峨ｒ譛ｬ譁・↓霑ｽ險倥＠縲ヽeport 縺ｫ docs/inbox/REPORT_...md 繧貞ｿ・★險ｭ螳・- Yarn Spinner縺ｮAPI縺梧Φ螳壹→逡ｰ縺ｪ繧句ｴ蜷医・縲∵怙譁ｰ縺ｮ繝峨く繝･繝｡繝ｳ繝医ｒ蜿ら・縺励※驕ｩ蛻・↓螳溯｣・☆繧・- MetaEffectController繧ДeductionBoard縺梧悴螳溯｣・・蝣ｴ蜷医・縲．ebug.Log縺ｧ莉｣譖ｿ縺励∝ｾ檎ｶ壹ち繧ｹ繧ｯ縺ｧ螳溯｣・☆繧区葎繧偵Ξ繝昴・繝医↓險倬鹸縺吶ｋ
- Prefab・・essageBubble, TypingIndicator・峨′譛ｪ菴懈・縺ｮ蝣ｴ蜷医・縲］ull繝√ぉ繝・け繧帝←蛻・↓陦後＞縲∬ｭｦ蜻翫Ο繧ｰ繧貞・蜉帙☆繧・
