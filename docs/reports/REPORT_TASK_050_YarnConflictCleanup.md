# Report: TASK_050 Yarn Conflict Cleanup

**Date**: 2026-02-17
**Status**: COMPLETED

## Summary
- `VerticalSlice.yarn` の開始ノードを `VerticalSlice_Start` に変更し、`Start` 重複を解消。
- `VerticalSlice.yarn` 側の `$has_topic_debug_topic_01` 宣言を削除し、変数再宣言衝突を解消。
- `VerticalSliceSceneSetup` / `DebugChatScene` / `VerticalSliceSmokeGatePlayModeTests` の開始ノード参照を `VerticalSlice_Start` に統一。
- `BuildSettingsHelper.cs` を修正し、TitleScene/DebugChatSceneをBuild Settingsに自動追加するよう変更。

## Changed Files
- `Assets/Resources/Yarn/VerticalSlice.yarn`
- `Assets/Scripts/Editor/VerticalSliceSceneSetup.cs`
- `Assets/Scenes/DebugChatScene.unity`
- `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`
- `Assets/Scripts/Editor/BuildSettingsHelper.cs`

## Verification

### Static Check: PASS
- `title: Start` は `DebugScript.yarn` のみ
- `title: VerticalSlice_Start` は `VerticalSlice.yarn`
- `$has_topic_debug_topic_01` 宣言は `DebugScript.yarn` のみ

### EditMode Runtime Check: PASS
- 実行日時: 2026-02-17T07:37:42Z
- 実行コマンド: `Unity.exe -batchmode -projectPath ... -quit`
- 結果: **return code 0** (正常終了)
- Yarnコンパイルエラー: **0件**
- ログ: `docs/evidence/TASK_050/EditorReimport_20260217.log`

### PlayMode Runtime Check: FAIL (Yarnスコープ外の問題)
- 実行日時: 2026-02-17T17:36:55+09:00
- テスト: `VerticalSliceSmokeGatePlayModeTests.VerticalSlice_SmokeFlow_TitleToChat_SaveLoad`
- 結果: **Failed**
- 失敗理由: `TitleScene: TitleScreenManager not found.`
- ログ: `docs/evidence/TASK_050/PlayModeTest_20260217_v3.log`
- 証跡: `docs/evidence/TASK_047/VerticalSliceSmokeGate_20260217_173655_TitleScene.txt`

**注記**: PlayModeテスト失敗はYarnスクリプトの問題ではなく、TitleSceneにTitleScreenManagerコンポーネントが存在しないことが原因。TASK_050のスコープ（Yarnスクリプト修正）は完了している。

## DoD Status
- [x] Start ノード重複解消（静的確認）
- [x] 変数再宣言衝突解消（静的確認）
- [x] 開始ノード一意化（静的確認）
- [x] EditMode実行証跡取得（Yarnコンパイルエラー0）
- [x] PlayMode実行証跡取得（失敗理由記録：Yarnスコープ外）

## Follow-up Tasks
- TitleScene の `TitleScreenManager` 参照切れ修正後に PlayMode テストを再実行し、`VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` の通過を確認する。
- 必要に応じて `Tools/FoundPhone/Re-Setup/Vertical Slice Essentials` を実行して BuildSettings/TitleScene 配線を再セットアップする。
