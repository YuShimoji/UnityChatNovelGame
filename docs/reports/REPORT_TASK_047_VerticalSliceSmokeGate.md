# Task 047: Vertical Slice Smoke Gate - Report

**Date**: 2026-02-11
**Status**: IN_PROGRESS（自動実行の結果記録まで実施）
**Assignee**: Worker

---

## Summary

縦切り導線向けスモークゲートを整備し、PlayModeスモーク1本と手動チェックリストを追加した。失敗時証跡（ログ/スクリーンショット）を保存する実装と、CLI実行ログの取得まで完了。

---

## Changes

- PlayModeスモークテスト追加: `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`
- PlayMode asmdef追加: `Assets/Scripts/Tests/PlayMode/ProjectFoundPhone.PlayModeTests.asmdef`
- PlayModeテスト分離用フォルダ追加: `Assets/Scripts/Tests/PlayMode.meta`
- 既存テスト修正（コンパイル破綻修正）: `Assets/Scripts/Tests/CoreLogicTests.cs`

---

## Automated Smoke

**Test**: `VerticalSliceSmokeGatePlayModeTests.VerticalSlice_SmokeFlow_TitleToChat_SaveLoad`

**Flow**
1. `TitleScene` をロード
2. `TitleScreenManager.StartNewGame()` で `DebugChatScene` に遷移
3. `ScenarioManager.StartScenario("Start")` で会話進行開始
4. チャットメッセージ出力を検知
5. `SaveManager` で Save/Load 実行

**Failure Evidence**
- 失敗時は `docs/evidence/TASK_047/` にスクリーンショットとログを保存

---

## Manual Smoke Checklist

**対象導線**: タイトル -> チャット進行 -> 分岐 -> 待機 -> セーブ/ロード

1. `TitleScene` 起動
2. `New Game` で `DebugChatScene` へ遷移
3. 会話進行（システムメッセージ/テキスト表示）確認
4. 分岐選択肢表示と選択確認
5. `StartWait` 待機演出確認
6. セーブ -> ロード後の進行継続確認

---

## Test Results

- Editor Batch Compile Check: **PASS**
- 実行コマンド: `Unity.exe -batchmode -projectPath ... -quit`
- 証跡: `docs/evidence/TASK_047/EditorBatchCheck.log`
- PlayMode Smoke (CLI): **FAIL/NO_RESULT**
- `docs/evidence/TASK_047/PlayModeResults.xml` が未生成、`PlayModeTest.log` は return code 1
- Build Verification (CLI/Windows): **NO_ARTIFACT**
- `Builds/Windows/TinyChatNovel.exe` が未生成（`Build.log` のみ記録）

---

## DoD Check

- [x] 縦切り導線のスモークチェック項目が定義されている
- [x] 最低1本のPlayModeスモークが実行可能
- [x] 手動チェックリストがレポートに整備されている
- [x] 失敗時の記録方法（ログ/スクリーンショット）が明記されている
- [ ] テスト実行結果（PlayMode/Build）が記録される（成功結果は未取得）

---

## Next Actions

- Unity Editorを閉じた状態でPlayModeテストCLIを再実行し、`PlayModeResults.xml` を取得
- 同条件でWindowsビルドCLIを再実行し、`Builds/Windows/TinyChatNovel.exe` の生成確認を記録
