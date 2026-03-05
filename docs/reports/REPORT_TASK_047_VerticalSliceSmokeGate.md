# Task 047: Vertical Slice Smoke Gate - Report

**Date**: 2026-02-22
**Status**: DONE
**Assignee**: Worker

---

## Summary

縦切り導線向けスモークゲートを整備し、PlayModeスモーク1本と手動チェックリストを追加した。失敗時証跡（ログ/スクリーンショット）を保存する実装と、CLI実行ログ及びビルド検証結果を完備した。Task 052にてテスト結果をクローズ済み。

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
- PlayMode Smoke (CLI): **PASS**
  - 実行コマンド完了、`docs/evidence/TASK_047/PlayModeResults.xml` を生成成功
  - `VerticalSliceSmokeGatePlayModeTests` はパス。※`MVPScene`は自動ロード無効化対応で除外。
- Build Verification (CLI/Windows): **PASS**
  - CLI実行によるビルドが成功
  - 証跡: `Builds/Windows/TinyChatNovel.exe` 及び生成ログ

---

## DoD Check

- [x] 縦切り導線のスモークチェック項目が定義されている
- [x] 最低1本のPlayModeスモークが実行可能
- [x] 手動チェックリストがレポートに整備されている
- [x] 失敗時の記録方法（ログ/スクリーンショット）が明記されている
- [x] テスト実行結果（PlayMode/Build）が記録される

---

## Next Actions

- なし（完了）
