# Task 016: Verification Tools (Automated Capture)

## Status
- [x] **DONE**

## Classification
- **Tier**: Tier 3 (Utility / Tooling)
- **Focus Area**: `Assets/Scripts/Utils/`

## Context
MCP迺ｰ蠅・ｸ九〒縺ｯ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ縺ｮ謦ｮ蠖ｱ縺悟峅髮｣縺ｧ縺ゅｊ縲∵焔蜍墓､懆ｨｼ縺後ヶ繝ｭ繝・き繝ｼ縺ｨ縺ｪ縺｣縺ｦ縺・ｋ縲・
縺薙ｌ繧定ｧ｣豸医☆繧九◆繧√￣layMode螳溯｡梧凾縺ｫ閾ｪ蜍慕噪縺ｫ險ｼ諡・医せ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ/繝ｭ繧ｰ・峨ｒ菫晏ｭ倥☆繧九Θ繝ｼ繝・ぅ繝ｪ繝・ぅ繧剃ｽ懈・縺吶ｋ縲・

## Requirements
1. **Script Creation**: `Assets/Scripts/Utils/VerificationCapture.cs`
   - `MonoBehaviour` 繧堤ｶ呎価縲・
   - `Start` 縺ｾ縺溘・迚ｹ螳壹・繧ｿ繧､繝溘Φ繧ｰ縺ｧ繧ｹ繧ｯ繝ｪ繝ｼ繝ｳ繧ｷ繝ｧ繝・ヨ繧呈聴蠖ｱ縲・
   - 菫晏ｭ伜・: `docs/evidence/` (蠢・・縲・
   - 繝輔ぃ繧､繝ｫ蜷・ `Capture_{Timestamp}_{SceneName}.png` 縺ｾ縺溘・謖・ｮ壹＆繧後◆蜷榊燕縲・
   - (Optional) `Application.logMessageReceived` 繧偵ヵ繝・け縺励※繝ｭ繧ｰ繧偵ユ繧ｭ繧ｹ繝井ｿ晏ｭ倥・

2. **Integration Test Support**:
   - 繝・せ繝医Λ繝ｳ繝翫・縺九ｉ縺薙・繧ｹ繧ｯ繝ｪ繝励ヨ繧貞性繧繧ｷ繝ｼ繝ｳ繧貞ｮ溯｡後＠縺滄圀繧ゅ√ヵ繧｡繧､繝ｫ縺檎函謌舌＆繧後ｋ縺薙→縲・

## Definition of Done (DoD)
- [x] `VerificationCapture.cs` 縺御ｽ懈・縺輔ｌ縺ｦ縺・ｋ縲・
- [x] 繝・せ繝医す繝ｼ繝ｳ縺ｫ驟咲ｽｮ縺励￣layMode螳溯｡悟ｾ後↓ `docs/evidence/` 縺ｫ逕ｻ蜒上ヵ繧｡繧､繝ｫ縺檎函謌舌＆繧後ｋ縺薙→繧堤｢ｺ隱阪・
- [x] 譌｢蟄倥・ `DebugChatScene` 縺ｪ縺ｩ縺ｫ邨・∩霎ｼ縺ｿ蜿ｯ閭ｽ縺ｧ縺ゅｋ縺薙→縲・

## Forbidden Area
- `Assets/Scripts/Core` (繝ｭ繧ｸ繝・け譛ｬ菴薙↓縺ｯ蠖ｱ髻ｿ繧剃ｸ弱∴縺ｪ縺・％縺ｨ)
