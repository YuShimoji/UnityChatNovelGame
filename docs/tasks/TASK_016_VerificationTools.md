# Task 016: Verification Tools (Automated Capture)

## Status
- [x] **DONE**

## Classification
- **Tier**: Tier 3 (Utility / Tooling)
- **Focus Area**: `Assets/Scripts/Utils/`

## Context
MCP環境下ではスクリーンショットの撮影が困難であり、手動検証がブロッカーとなっている。
これを解消するため、PlayMode実行時に自動的に証拠（スクリーンショット/ログ）を保存するユーティリティを作成する。

## Requirements
1. **Script Creation**: `Assets/Scripts/Utils/VerificationCapture.cs`
   - `MonoBehaviour` を継承。
   - `Start` または特定のタイミングでスクリーンショットを撮影。
   - 保存先: `Docs/evidence/` (必須)。
   - ファイル名: `Capture_{Timestamp}_{SceneName}.png` または指定された名前。
   - (Optional) `Application.logMessageReceived` をフックしてログをテキスト保存。

2. **Integration Test Support**:
   - テストランナーからこのスクリプトを含むシーンを実行した際も、ファイルが生成されること。

## Definition of Done (DoD)
- [x] `VerificationCapture.cs` が作成されている。
- [x] テストシーンに配置し、PlayMode実行後に `docs/evidence/` に画像ファイルが生成されることを確認。
- [x] 既存の `DebugChatScene` などに組み込み可能であること。

## Forbidden Area
- `Assets/Scripts/Core` (ロジック本体には影響を与えないこと)
