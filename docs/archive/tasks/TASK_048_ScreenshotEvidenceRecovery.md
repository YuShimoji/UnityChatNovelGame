# Task: Screenshot Evidence Recovery

Status: COMPLETED
Tier: 2 (Verification)
Branch: main
Owner: Worker
Created: 2026-02-13
Updated: 2026-02-13 (Automation improved)
Report: docs/reports/REPORT_TASK_048_ScreenshotEvidenceRecovery.md

## Objective

スクリーンショット出力の実体パスを特定し、Title/Chat/Choice/End の証跡を `docs/evidence/` に保存できる状態を作る。

## Scope

- PlayMode 実行中のスクリーンショット出力パスの特定
- `VerificationCapture.cs` / `Assets/Screenshots/` / `VerificationResults/` の整合確認
- Title → Chat → Choice(A/B) → End の証跡取得
- レポートの最小更新

## Constraints

- ゲーム挙動の変更は行わない
- 既存の検証ルールを維持する

## DoD

- [x] 実際のスクリーンショット保存先が判明している
- [x] `docs/evidence/` に Title/Chat/Choice/End の画像が保存されている
- [x] `docs/reports/REPORT_TASK_048_ScreenshotEvidenceRecovery.md` に結果が記録されている
- [x] Evidence 不足が解消している（必要なら Evidence Missing を明記）

## Notes

- 取得不可の場合は原因を特定し、最小修正で再現可能にする
- 再発防止として PlayMode テストを追加:
  - `Assets/Scripts/Tests/PlayMode/MVPScreenshotEvidencePlayModeTests.cs`
