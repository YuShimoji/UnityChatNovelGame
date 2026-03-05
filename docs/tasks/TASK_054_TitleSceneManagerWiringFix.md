# Task: TitleScene Manager Wiring Fix for Smoke Flow

Status: DONE (Verification Bypassed)
Tier: 1 (Hotfix)
Branch: feature/task-054-title-scene-manager-wiring-fix
Owner: Worker
Created: 2026-02-19
Updated: 2026-02-24
Report: docs/reports/REPORT_TASK_054_TitleSceneManagerWiringFix.md
Notes: ユーザー依頼により手動検証をパスし、開発を優先。代わりに VerificationAutomator.cs による自動化基盤を導入。

## Objective

`VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` の失敗原因（`TitleScene: TitleScreenManager not found`）を解消し、縦切りスモークフローを Green 化する。

## Milestone

- SG-1: MVP縦切りの最終確認
- MG-1: MVP安定化と最低限の品質基盤

## Target Assemblies

- `ProjectFoundPhone`
- `ProjectFoundPhone.PlayModeTests`

## Focus Area

- `Assets/Scenes/TitleScene.unity`
- `Assets/Scripts/UI/TitleScreenManager.cs`
- `Assets/Scripts/Editor/BuildSettingsHelper.cs`
- `Assets/Scripts/Tests/PlayMode/VerticalSliceSmokeGatePlayModeTests.cs`
- `docs/evidence/TASK_047/`

## Forbidden Area

- 新規機能の追加
- 監査範囲外の大規模リファクタ
- 演出仕様の追加決定

## Constraints

- 対象外アセンブリに変更が必要になった場合は `BLOCKED` として停止する
- 既存の Re-Setup/Validate メニューは壊さない
- 原因推測だけで完了扱いにしない（再実行証跡を必須化）

## DoD

- [ ] `VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` が PASS する
- [ ] `docs/evidence/TASK_047/PlayModeResults.xml` が更新され、対象2テストが PASS である
- [ ] Unity Editor でコンパイルエラー 0 を確認する
- [ ] 変更内容と根拠を `docs/reports/REPORT_TASK_054_TitleSceneManagerWiringFix.md` に記録する

## Test Plan

- テスト対象:
  - `VerticalSliceSmokeGatePlayModeTests`
  - `TitleScene` の `TitleScreenManager` 配線
- テスト種別:
  - EditMode（コンパイル確認）
  - PlayMode（CLI）
- 期待結果:
  - `TitleScene: TitleScreenManager not found` が再発しない
  - スモーク2テストがともに PASS
- テスト不要項目:
  - 新規ユニットテスト追加

## Stop Conditions

- シーン構成破損により復旧手順が必要
- asmdef 境界を越える変更が必要
- 実行環境依存で PlayMode CLI 再現が不可

## Update (2026-02-19 Interim)

- `Assets/Scripts/Editor/TitleSceneSetupTools.cs` を更新し、`EnsureTitleSceneManager` を `internal` 化して再利用可能にした。
- `SceneSetupResult` に `ComponentAdded` / `AssignedDefaultScene` / `SceneModified` を追加し、変更有無を明示できるようにした。
- `TitleScreenManager` と `m_NewGameSceneName`（`DebugChatScene`）の補完処理を追加し、実際に変更があった場合のみシーンを save する実装にした。
- 未完了:
  - Re-Setup/Validate 実行結果の採取
  - PlayMode CLI 再実行（`VerticalSlice_SmokeFlow_TitleToChat_SaveLoad` PASS確認）
  - 証跡・レポート最終化
