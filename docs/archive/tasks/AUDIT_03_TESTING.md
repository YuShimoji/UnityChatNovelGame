# AUDIT_03: テスト・品質保証

**点検日**: 2026-02-08
**テストフレームワーク**: Unity Test Framework 2.0.1

---

## 現状のテストカバレッジ

| モジュール | テストファイル | テスト数 | 種別 |
|-----------|-------------|---------|------|
| SaveSystem | `SaveSystemTests.cs` | 6 | EditMode |
| TopicData | `CoreLogicTests.cs` | 4 | EditMode |
| SynthesisRecipe | `CoreLogicTests.cs` | 4 | EditMode |
| DeductionBoard (ロジック) | `CoreLogicTests.cs` | 7 | EditMode |
| SaveData シリアライズ | `CoreLogicTests.cs` | 3 | EditMode |
| **合計** | — | **24** | — |

**推定カバレッジ**: 約 20%（コアデータ層のみ。UI・シナリオ・エフェクト層はゼロ）

---

## QA-01: ChatController のテストが皆無 🔴

- **対象**: `Assets/Scripts/UI/ChatController.cs` (628行)
- **内容**: AddMessage / AddImageMessage / AddSystemMessage / ShowChoices / AutoScroll のいずれもテストなし
- **リスク**: UI ロジックの回帰バグを検知できない
- **提案**: PlayMode テストで基本的なメッセージ追加・スクロール動作を検証。最低 5 ケース

---

## QA-02: ScenarioManager のテストが皆無 🔴

- **対象**: `Assets/Scripts/Core/ScenarioManager.cs` (502行)
- **内容**: カスタムコマンド（Message, Image, UnlockTopic, Glitch, SystemMessage, StartWait）のテストなし
- **リスク**: Yarn Spinner 連携の回帰バグ
- **提案**: DialogueRunner のモック or PlayMode テストで各コマンドの動作を検証。最低 6 ケース

---

## QA-03: MetaEffectController / GlitchEffect のテストが皆無 🟡

- **対象**: `Assets/Scripts/Effects/MetaEffectController.cs`, `GlitchEffect.cs`
- **内容**: エフェクト再生・レベル切替のテストなし
- **リスク**: エフェクト演出の不具合が検知できない
- **提案**: PlayMode テストでエフェクト起動・停止を検証。最低 3 ケース

---

## QA-04: TitleScreenManager のテストが皆無 🟡

- **対象**: `Assets/Scripts/UI/TitleScreenManager.cs` (2423 bytes)
- **内容**: シーン遷移・ボタン動作のテストなし
- **提案**: PlayMode テストでボタンクリック→シーン遷移を検証

---

## QA-05: DeductionBoard の統合テストが不十分 🟡

- **対象**: `Assets/Scripts/UI/DeductionBoard.cs`
- **内容**: `CoreLogicTests.cs` にロジックテスト (7件) はあるが、UI 生成（TopicCard の Instantiate）や D&D 合成の統合テストがない
- **補足**: `DeductionBoardSynthesisTest.cs` と `DeductionBoardVerification.cs` は検証スクリプト（NUnit テストではない）
- **提案**: PlayMode テストで AddTopic → カード生成 → 合成フローを検証

---

## QA-06: SaveLoadUI / SaveSlotUI のテストが皆無 🟡

- **対象**: `Assets/Scripts/UI/SaveLoadUI.cs`, `SaveSlotUI.cs`
- **内容**: セーブ/ロード UI のインタラクションテストなし
- **提案**: PlayMode テストでスロット選択→セーブ→ロードのフローを検証

---

## QA-07: CharacterDatabase / CharacterProfile のテストが皆無 🟡

- **対象**: `Assets/Scripts/Data/CharacterDatabase.cs`, `CharacterProfile.cs`
- **内容**: プロファイルのロード・検索・フォールバック動作のテストなし
- **提案**: EditMode テストで GetProfile / IsPlayer / GetThemeColor のフォールバック含め検証。最低 4 ケース

---

## QA-08: Full Playthrough テストが手動待ち 🟠

- **対象**: TASK_027
- **内容**: 統合動作検証が Unity Editor での手動テスト待ちのまま長期間放置
- **リスク**: 全体の統合品質が未確認
- **提案**: 最低限 DebugChatScene を起動し、基本フローを通す。結果を `docs/evidence/` に記録

---

## テスト拡充ロードマップ（推奨順序）

1. **CharacterDatabase EditMode テスト** — 依存が少なく、すぐ書ける
2. **ChatController PlayMode テスト** — 最大のテストギャップ
3. **ScenarioManager PlayMode テスト** — コマンド連携の検証
4. **TASK_027 手動テスト実行** — 統合品質の確認
5. **MetaEffectController PlayMode テスト** — エフェクト層の検証
