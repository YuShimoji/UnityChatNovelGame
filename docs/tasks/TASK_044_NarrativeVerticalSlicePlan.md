# Task: Narrative Vertical Slice Plan (Mock Story Playable)

Status: DONE
Tier: 1 (Planning/Execution Guide)
Branch: main
Owner: Orchestrator
Created: 2026-02-09
Updated: 2026-03-01T06:10:00+09:00
Report: docs/reports/REPORT_TASK_044_NarrativeVerticalSlice.md

## Objective

Yarn ベースのモックストーリーを `DebugChatScene` で実際にプレイ可能な状態へ到達させるための計画を固定し、その達成を確認する。

## Milestone Definition

- M1 (Narrative Vertical Slice):
  - `Assets/Resources/Yarn/VerticalSlice.yarn` の開始ノードから終了まで進行できる
  - 分岐を 1 回以上含む
  - `ChatDialogueView` と `ChatController` 経由で行表示・選択肢表示が機能する
  - `DebugChatScene` で通しプレイが可能

## Decision

- 採用継続: Yarn Spinner
- 理由:
  1. `ScenarioManager` / `ChatDialogueView` / custom command 群が Yarn 前提で整備済み
  2. `VerticalSlice.yarn` と batch verification が既に機能している
  3. エンジン移行は M1 達成後の別判断でよい

## Current Status

- `VerticalSlice.yarn` は `VerticalSlice_Start` から `End` まで 2 分岐を含む構成で実装済み
- `TASK_027` full playthrough batch が SUCCESS
- `TASK_047` / `TASK_052` smoke gate で Title -> Chat -> Save/Load が PASS
- `TASK_053` で narrative slice を含む MVP verification pack が CLOSE

## DoD (Definition of Done)

- [x] Narrative vertical slice の到達条件が明文化されている
- [x] Yarn Spinner を SSOT とする判断が固定されている
- [x] `DebugChatScene` で開始から終了までのプレイ可能経路が存在する
- [x] 分岐と終端が latest evidence で確認されている
- [x] `docs/reports/REPORT_TASK_044_NarrativeVerticalSlice.md` に完了根拠が記録されている

## Evidence

- `Assets/Resources/Yarn/VerticalSlice.yarn`
- `docs/evidence/TASK_027/FULL_PLAYTHROUGH_RESULTS_20260301_034145.md`
- `docs/reports/REPORT_TASK_027_FullPlaythroughTest.md`
- `docs/reports/REPORT_TASK_047_VerticalSliceSmokeGate.md`
- `docs/reports/REPORT_TASK_052_VerticalSliceSmokeResultClosure.md`
- `docs/reports/REPORT_TASK_053_MVPFinalVerificationPack.md`

## Follow-up

- 次の narrative 拡張は本タスクではなく、次サイクルの content planning として扱う
