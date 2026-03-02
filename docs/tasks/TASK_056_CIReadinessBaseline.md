# TASK_056_CIReadinessBaseline

Status: IN_PROGRESS

## Objective
LG-1 の最初の slice として、remote 上で最低限の repository guard を自動実行できる CI baseline を整備する。

## Context
- Addressables は移行面積が大きく、Phase 1 の最初の 1 本としては重い。
- QA 強化は重要だが、Unity 実行依存と手動確認の割合がまだ高い。
- 先に CI baseline を敷くと、以後の docs / workflow / report 整流を remote 側でも継続検証できる。

## Focus Area
- `.github/workflows/repo-guards.yml`
- `docs/HANDOVER.md`
- `docs/inbox/REPORT_ORCH_*.md`
- `.shared-workflows/scripts/report-validator.js`
- `.shared-workflows/scripts/session-end-check.js`

## Layer A
- [x] CI baseline の候補比較を実施し、Addressables / QA ではなく CI を先行 slice に選定する
- [x] GitHub Actions workflow を追加する
- [x] handover / latest orchestrator report / session-end-check を remote guard として組み込む
- [x] local で workflow 対象スクリプトの syntax / validator 実行を確認する

## Layer B
- [ ] remote push 後に GitHub Actions の初回 green run を確認する

## Definition of Done
- [x] `.github/workflows/repo-guards.yml` が追加されている
- [x] CI baseline の意図と範囲が task / report / SSOT に記録されている
- [ ] remote 上で workflow が 1 回成功している

## Milestone
- LG-1: Production readiness

## Stop Conditions
- GitHub Actions 実行権限や repository settings が不足して remote run が開始できない
- `.shared-workflows` submodule の参照解決が remote checkout で失敗する
