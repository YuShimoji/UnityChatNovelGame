# REPORT_TASK_054: TitleScene Manager Wiring Fix

- **Task:** TASK_054
- **Date:** 2026-02-19
- **Author:** Orchestrator Follow-up
- **Branch:** feature/task-049-build-gate-fix
- **Status:** 🚧 IN_PROGRESS

---

## 概要

`VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` の失敗（`TitleScene: TitleScreenManager not found`）解消に向けて、
TitleScene 再セットアップ処理の再利用性と差分保存制御を強化した。

---

## 実施済み

1. `Assets/Scripts/Editor/TitleSceneSetupTools.cs`
   - `EnsureTitleSceneManager` を `private` から `internal` に変更
   - `SceneSetupResult` を拡張（`ComponentAdded` / `AssignedDefaultScene` / `SceneModified`）
   - `TitleScreenManager` の自動補完
   - `m_NewGameSceneName` が空の場合に `DebugChatScene` を自動設定
   - 実際に変更がある場合のみ `MarkSceneDirty` / `SaveScene` を実行

2. 既存レポート整合
   - `REPORT_TASK_047` / `REPORT_TASK_052` の FAIL 原因表現を
     `TitleScene: TitleScreenManager not found` に統一

---

## 未実施（次担当向け）

1. Unity Editor で以下を実行
   - `Tools/FoundPhone/Re-Setup/TitleScene Manager`
   - `Tools/FoundPhone/Validate/TitleScene Manager Wiring`

2. PlayMode CLI 再実行
   - `VerticalSliceSmokeGatePlayModeTests` を実行し、`VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` の PASS 可否を確認

3. 証跡更新
   - `docs/evidence/TASK_047/PlayModeResults.xml`
   - 必要なら `docs/evidence/TASK_047/PlayModeTest_054.log`

4. DoD 最終反映
   - `docs/tasks/TASK_054_TitleSceneManagerWiringFix.md` の DoD チェック
   - `Status: DONE` への更新（条件達成時のみ）

---

## 引き継ぎメモ

- 既存ワークツリーには他タスク由来の変更が混在しているため、コミット時は `TASK_054` 関連ファイルを明示選択する。
- `InitTestScene...` など一時生成ファイルがあるため、不要なら別コミットに分離する。
