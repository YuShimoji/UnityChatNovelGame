# Implementation Plan - Deduction Board & Task Cleanup

## Goal Description
驥崎､・＠縺ｦ縺・ｋ Deduction Board 髢｢騾｣繧ｿ繧ｹ繧ｯ (TASK_016, TASK_017) 繧堤ｵｱ蜷医＠縲∝腰荳縺ｮ譏守｢ｺ縺ｪ螳溯｣・ち繧ｹ繧ｯ (TASK_018) 繧貞ｮ夂ｾｩ縺吶ｋ縲・縺昴・蠕後．eduction Board 縺ｮ UI 縺翫ｈ縺ｳ繝ｭ繧ｸ繝・け螳溯｣・ｒ陦後＞縲～UnlockTopicCommand` 縺ｨ縺ｮ騾｣謳ｺ繧貞ｮ御ｺ・＆縺帙ｋ縲・
## User Review Required
- **Task Consolidation**: TASK_016 縺ｨ TASK_017 縺ｯ邨ｱ蜷医＆繧後，lose 縺輔ｌ縺ｾ縺吶・- **New Task**: TASK_018_DeductionBoard_Implementation 縺梧眠縺溘↓ SSOT 縺ｨ縺ｪ繧翫∪縺吶・
## Proposed Changes

### 1. Task Management
#### [NEW] [TASK_018_DeductionBoard_Implementation.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/tasks/TASK_018_DeductionBoard_Implementation.md)
- TASK_016 縺ｨ TASK_017 縺ｮ隕∽ｻｶ繧偵・繝ｼ繧ｸ縲・- **Scope**:
  - `DeductionBoard.cs`: 繝医ヴ繝・け邂｡逅・ゞI蛻ｶ蠕｡
  - `TopicCard.cs`: 繝医ヴ繝・け蛟句挨陦ｨ遉ｺ
  - Prefabs: `DeductionBoard.prefab`, `TopicCard.prefab`
  - Integration: `ScenarioManager` 縺九ｉ縺ｮ繧､繝吶Φ繝亥女菫｡ (`UnlockTopic`)

#### [MODIFY] [TASK_016_DeductionBoard_Conflict.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/tasks/TASK_016_DeductionBoard_Conflict.md)
- Status: **CLOSED (Merged to TASK_018)**

#### [MODIFY] [TASK_017_DeductionBoard_Conflict.md](file:///c:/Users/thank/Storage/Game%20Projects/UnityChatNovelGame/docs/tasks/TASK_017_DeductionBoard_Conflict.md)
- Status: **CLOSED (Merged to TASK_018)**

### 2. Implementation (TASK_018)
#### [NEW] Assets/Scripts/UI/DeductionBoard.cs
- `MonoBehaviour`
- Singleton pattern (or reference injection)
- Methods: `AddTopic(TopicData)`, `HasTopic(string id)`
- UI Elements: `ScrollView`, `Content` (Grid Layout)

#### [NEW] Assets/Scripts/UI/TopicCard.cs
- `MonoBehaviour`
- Methods: `Setup(TopicData)`
- UI Elements: `Image (Icon)`, `TextMeshProUGUI (Title)`

#### [MODIFY] Assets/Scripts/Core/ScenarioManager.cs
- `UnlockTopicCommand` 蜀・〒 `DeductionBoard.Instance.AddTopic()` 繧貞他縺ｳ蜃ｺ縺吝・逅・ｒ豁｣蠑丞ｮ溯｣・ｼ育樟蝨ｨ縺ｯ Debug.Log 縺ｮ縺ｿ縺ｮ蝣ｴ蜷医≠繧奇ｼ峨・
## Verification Plan

### Automated Tests
- **EditMode Test**: `TopicCard` 縺ｮ逕滓・縺ｨ繝・・繧ｿ蜑ｲ繧雁ｽ薙※縺ｮ繝・せ繝茨ｼ・refab萓晏ｭ倥・縺溘ａ PlayMode 縺碁←蛻・°繧ゑｼ峨・- **PlayMode Test**: `DeductionBoard` 縺ｫ繝医ヴ繝・け繧定ｿｽ蜉縺励ゞI隕∫ｴ縺悟｢励∴繧九％縺ｨ繧堤｢ｺ隱阪・
### Manual Verification
1. **Scene**: `Assets/Scenes/DebugChatScene.unity`
2. **Action**:
   - Play 繧ｷ繝ｼ繝ｳ繧貞ｮ溯｡後・   - `<<UnlockTopic>>` 繧ｳ繝槭Φ繝峨′螳溯｡後＆繧後ｋ縺ｾ縺ｧ騾ｲ繧√ｋ縲・3. **Expectation**:
   - 逕ｻ髱｢荳奇ｼ医∪縺溘・蟆ら畑縺ｮ繧ｪ繝ｼ繝舌・繝ｬ繧､・峨↓ Deduction Board 縺瑚｡ｨ遉ｺ縺輔ｌ・医∪縺溘・繝懊ち繝ｳ縺ｧ髢矩哩・峨∵眠縺励＞繝医ヴ繝・け繧ｫ繝ｼ繝峨′霑ｽ蜉縺輔ｌ縺ｦ縺・ｋ縺薙→縲・
