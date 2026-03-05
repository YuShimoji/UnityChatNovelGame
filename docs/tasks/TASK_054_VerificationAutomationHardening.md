# Task: Verification Automation Hardening

Status: COMPLETED
Tier: 2 (Stabilization)
Branch: main
Owner: Orchestrator
Created: 2026-03-01T03:30:00+09:00
Updated: 2026-03-01T05:20:00+09:00
Report: docs/reports/REPORT_TASK_054_VerificationAutomationHardening.md

## Objective

MVP / Vertical Slice / Performance の batch verification を安定化し、再実行時のノイズと residual warning を減らして証跡品質を引き上げる。

## Milestone

- MG-1: MVP 安定化と最低限の品質基盤

## Focus Area

- `Assets/Scripts/Automation/VerificationAutomator.cs`
- `Assets/Scripts/Editor/VerificationMenu.cs`
- `Assets/Scripts/Editor/MissingScriptScanner.cs`
- `Assets/Prefabs/UI/MessageBubble.prefab`
- `docs/logs/unity_missing_script_scan_20260301.log`
- `docs/logs/unity_missing_script_deps_20260301.log`
- `docs/logs/unity_missing_script_cleanup_20260301.log`
- `docs/logs/unity_performance_20260301_round6.log`

## Forbidden Area

- 新機能追加
- シナリオ本文の変更
- 手動テスト前提への後退

## Constraints

- Unity batch で再現できる範囲を優先する
- residual warning は source attribution まで行う
- 変更理由と証跡を report に残す

## DoD (Definition of Done)

- [x] batch screenshot capture のノイズを低減できる
- [x] `TASK_MVP_04` / `TASK_027` / `TASK_025` を batch ベースで再実行できる
- [x] `missing script` の発生源を source attribution できる
- [x] cleanup 後の subsequent run で raw warning 消失を確認できる
- [x] hardening 内容を `docs/reports/REPORT_TASK_054_VerificationAutomationHardening.md` に記録している

## Test Plan

- テスト対象:
  - MVP final verification pack
  - full playthrough batch
  - performance measurement batch
  - missing script scan / cleanup batch
- テスト種別:
  - Unity batch execution
  - log verification
- 期待結果:
  - `ReadPixels...` ノイズが消える
  - `MessageBubble.prefab` の residual missing MonoBehaviour が解消される
  - `TASK_025` の latest after measurement が成功する

## Stop Conditions

- Unity batch 自体が再現不能
- residual warning が第三者変更起因で安定再現しない
- cleanup により runtime 退行が発生する
